using Loretta.CodeAnalysis;
using Tsu;

namespace LuaPack.Core;

internal static class Helpers
{
    /// <summary>
    /// Resolves a require's path to be relative from the project's root so we have the same path everywhere.
    /// </summary>
    /// <param name="root">The project's root.</param>
    /// <param name="tree">The tree that contains the import.</param>
    /// <param name="name">The import path.</param>
    /// <returns></returns>
    public static Result<string, FileOutOFWorkspaceError> ResolveRequire(string root, SyntaxTree tree, string name)
    {
        var fullPath = Path.GetFullPath(name, Path.GetDirectoryName(tree.FilePath)!);
        var relative = MakePathRelativeToRoot(root, fullPath);
        if (relative.StartsWith("..", StringComparison.Ordinal))
        {
            return Result.Err<string, FileOutOFWorkspaceError>(new(
                MakePathRelativeToRoot(root, tree.FilePath),
                name));
        }
        return Result.Ok<string, FileOutOFWorkspaceError>(relative);
    }

    /// <summary>
    /// Makes an ABSOLUTE path relative to the project's root.
    /// </summary>
    /// <param name="root">The project's root.</param>
    /// <param name="path">The import's ABSOLUTE path.</param>
    /// <returns></returns>
    public static string MakePathRelativeToRoot(string root, string path)
    {
        var rootUri = new Uri(root, UriKind.Absolute);
        var pathUri = new Uri(path, UriKind.Absolute);
        var relativeUri = rootUri.MakeRelativeUri(pathUri);
        return relativeUri.ToString();
    }
}
