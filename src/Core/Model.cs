using System.Collections.Immutable;
using Loretta.CodeAnalysis;

namespace LuaPack.Core;

internal sealed record FileIncludeResult(string Path, SyntaxTree RewrittenTree, ImmutableArray<Import> References, ImmutableArray<Diagnostic> Diagnostics)
{
    public bool HasErrors => Diagnostics.Any(diag => diag.Severity == DiagnosticSeverity.Error);
}

internal sealed record Import(Location Location, string Path);

internal record FileOutOFWorkspaceError(string FilePath, string ImportPath);
