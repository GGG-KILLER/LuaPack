using System.Collections.Immutable;
using Loretta.CodeAnalysis;
using Loretta.CodeAnalysis.Lua;
using Loretta.CodeAnalysis.Lua.Syntax;
using Loretta.CodeAnalysis.Text;
using Tsu;

namespace LuaPack.Core;

public sealed class PackGenerator : IDisposable
{
    private readonly ReaderWriterLockSlim _includedFilesLock = new();
    private readonly List<FileIncludeResult> _includedFiles = new();
    private readonly string _projectRoot;
    private readonly LuaParseOptions _parseOptions;
    private readonly string _entryPoint;
    private readonly bool _cacheIncludes;

    public PackGenerator(
        string projectRoot,
        LuaParseOptions parseOptions,
        string entryPoint,
        bool cacheIncludes)
    {
        if (string.IsNullOrWhiteSpace(projectRoot)) throw new ArgumentException($"'{nameof(projectRoot)}' cannot be null or whitespace.", nameof(projectRoot));
        if (string.IsNullOrWhiteSpace(entryPoint)) throw new ArgumentException($"'{nameof(entryPoint)}' cannot be null or whitespace.", nameof(entryPoint));

        _projectRoot = projectRoot;
        _parseOptions = parseOptions ?? throw new ArgumentNullException(nameof(parseOptions));
        _entryPoint = entryPoint;
        _cacheIncludes = cacheIncludes;
    }

    public IEnumerable<Diagnostic> GetDiagnostics()
    {
        _includedFilesLock.EnterReadLock();
        try
        {
            foreach (var file in _includedFiles)
            {
                foreach (var diagnostic in file.RewrittenTree.GetDiagnostics())
                {
                    yield return diagnostic;
                }

                foreach (var diagnostic in file.Diagnostics)
                {
                    yield return diagnostic;
                }
            }
        }
        finally
        {
            _includedFilesLock.ExitReadLock();
        }
    }

    public Result<Unit, ImmutableArray<Diagnostic>> AddFile(string path, SourceText sourceText)
    {
        var tree = LuaSyntaxTree.ParseText(sourceText, _parseOptions, path);

        var treeDiags = tree.GetDiagnostics();
        if (treeDiags.Any())
            return Result.Err<Unit, ImmutableArray<Diagnostic>>(treeDiags.ToImmutableArray());

        var rewrittenRoot = ImportRewriter.Rewrite(
            _projectRoot,
            tree,
            out var imports,
            out var diagnostics);

        var relativePath = Helpers.MakePathRelativeToRoot(_projectRoot, path);
        var result = new FileIncludeResult(
            relativePath,
            tree.WithRootAndOptions(rewrittenRoot, _parseOptions),
            imports,
            diagnostics);

        _includedFilesLock.EnterWriteLock();
        try
        {
            _includedFiles.Add(result);
        }
        finally
        {
            _includedFilesLock.ExitWriteLock();
        }

        if (diagnostics.Any())
            return Result.Err<Unit, ImmutableArray<Diagnostic>>(diagnostics);
        else
            return Result.Ok<Unit, ImmutableArray<Diagnostic>>(Unit.Value);
    }

    private ImmutableArray<Import> ListUnresolvedImports(FileIncludeResult file)
    {
        var hasReadLock = _includedFilesLock.IsReadLockHeld;
        if (!hasReadLock)
            _includedFilesLock.EnterReadLock();

        try
        {
            return file.References.ExceptBy(_includedFiles.Select(x => x.Path), x => x.Path)
                                  .ToImmutableArray();
        }
        finally
        {
            if (!hasReadLock)
                _includedFilesLock.ExitReadLock();
        }
    }

    public Result<Unit, ImmutableArray<Diagnostic>> WriteOutput(TextWriter writer)
    {
        var statements = new List<StatementSyntax>();
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        statements.AddRange(_cacheIncludes ? SyntaxConstants.CachingHeader : SyntaxConstants.NonCachingHeader);

        _includedFilesLock.EnterReadLock();
        try
        {
            foreach (var file in _includedFiles)
            {
                diagnostics.AddRange(file.Diagnostics);
                var unresolvedImports = ListUnresolvedImports(file);
                foreach (var import in unresolvedImports)
                {
                    diagnostics.Add(Diagnostic.Create(
                        Diagnostics.UnresolvedReference,
                        import.Location,
                        import.Path));
                }

                statements.Add(SyntaxFactory.AssignmentStatement(
                    SyntaxFactory.SingletonSeparatedList<PrefixExpressionSyntax>(
                        SyntaxFactory.ElementAccessExpression(
                            SyntaxFactory.IdentifierName(SyntaxConstants.ImportFileTableName),
                            SyntaxFactoryEx.StringExpression(file.Path))),
                    SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                        SyntaxFactory.AnonymousFunctionExpression(
                            SyntaxFactory.ParameterList(
                                SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                    SyntaxFactory.VarArgParameter())),
                            file.RewrittenTree.GetCompilationUnitRoot().Statements))));
            }

            var entryPointPath = Helpers.MakePathRelativeToRoot(_projectRoot, Path.GetFullPath(_entryPoint, _projectRoot));
            if (entryPointPath.StartsWith("..", StringComparison.Ordinal))
            {
                diagnostics.Add(Diagnostic.Create(
                    Diagnostics.EntryPointOutOfWorkspace,
                    Location.None,
                    _entryPoint));
            }

            var entryPointFile = _includedFiles.SingleOrDefault(
                file => file.Path.Equals(entryPointPath, StringComparison.Ordinal));
            if (entryPointFile is null)
            {
                diagnostics.Add(Diagnostic.Create(
                    Diagnostics.UnresolvedEntryPoint,
                    Location.None,
                    _entryPoint));
            }

            statements.Add(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.FunctionCallExpression(
                    SyntaxConstants.ImportFunctionNameSyntax,
                    SyntaxFactoryEx.StringArgument(entryPointPath))));
        }
        finally
        {
            _includedFilesLock.ExitReadLock();
        }

        if (diagnostics.Any())
        {
            return Result.Err<Unit, ImmutableArray<Diagnostic>>(diagnostics.ToImmutable());
        }
        else
        {
            var compilationUnit = SyntaxFactory.CompilationUnit(
                SyntaxFactory.StatementList(statements));
            compilationUnit = compilationUnit.NormalizeWhitespace("    ", Environment.NewLine);
            compilationUnit.WriteTo(writer);
            return Result.Ok<Unit, ImmutableArray<Diagnostic>>(Unit.Value);
        }
    }

    public void Dispose() => ((IDisposable) _includedFilesLock).Dispose();
}
