namespace LuaPack;

internal static class PathHelper
{
    /// <summary>
    /// Makes an ABSOLUTE path relative to another ABSOLUTE path.
    /// </summary>
    /// <param name="basePath">The ABSOLUTE base path.</param>
    /// <param name="path">The import's ABSOLUTE path.</param>
    /// <returns></returns>
    public static string MakePathRelative(string basePath, string path)
    {
        var rootUri = new Uri(basePath, UriKind.Absolute);
        var pathUri = new Uri(path, UriKind.Absolute);
        var relativeUri = rootUri.MakeRelativeUri(pathUri);
        return relativeUri.ToString();
    }
}
