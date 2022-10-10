using System.Collections.Immutable;
using Loretta.CodeAnalysis;
using Loretta.CodeAnalysis.Lua;
using Loretta.CodeAnalysis.Lua.Syntax;

namespace LuaPack.Core;

internal class ImportRewriter : LuaSyntaxRewriter
{
    private readonly ImmutableArray<Import>.Builder _imports = ImmutableArray.CreateBuilder<Import>();
    private readonly ImmutableArray<Diagnostic>.Builder _diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
    private readonly string _root;

    private ImportRewriter(string root)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentException($"'{nameof(root)}' cannot be null or whitespace.", nameof(root));
        }

        _root = root;
    }

    public override SyntaxNode? VisitFunctionCallExpression(FunctionCallExpressionSyntax node)
    {
        if (SyntaxFactsEx.IsImport(node, out string? path))
        {
            var result = Helpers.ResolveRequire(_root, node.SyntaxTree, path);
            if (result.Err is { IsSome: true, Value: var err })
            {
                _diagnostics.Add(Diagnostic.Create(
                    Diagnostics.FileOutOfWorkspace,
                    node.GetLocation(),
                    err.ImportPath));
            }
            else
            {
                var resolvedPath = result.Ok.Value;
                _imports.Add(new(node.GetLocation(), resolvedPath));
                return node.Update(
                    SyntaxConstants.ImportFunctionNameSyntax
                        .WithLeadingTrivia(node.Expression.GetLeadingTrivia())
                        .WithTrailingTrivia(SyntaxFactory.Space),
                    SyntaxFactory.StringFunctionArgument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(resolvedPath)))
                        .WithTriviaFrom(node.Argument));
            }
        }
        else if (SyntaxFactsEx.IsImportName(node.Expression)
            && node.Argument is ExpressionListFunctionArgumentSyntax { Expressions.Count: 1 })
        {
            return node.Update(
                SyntaxConstants.ImportFunctionNameSyntax.WithTriviaFrom(node.Expression),
                node.Argument);
        }

        return base.VisitFunctionCallExpression(node);
    }

    public static SyntaxNode Rewrite(string root, SyntaxTree tree, out ImmutableArray<Import> imports, out ImmutableArray<Diagnostic> diagnostics)
    {
        ImportRewriter finder = new(root);
        var node = finder.Visit(tree.GetRoot());
        imports = finder._imports.ToImmutable();
        diagnostics = finder._diagnostics.ToImmutable();
        return node;
    }
}
