namespace LuaPack.Core;

/// <summary>
/// Indicates that a file imports a file that is not part of the pack.
/// </summary>
/// <param name="SourceFile">The path of the file that contains the missing import.</param>
/// <param name="ImportPath">The file that the file at <paramref name="ImportPath"/> is attempting to import.</param>
public sealed record MissingReferenceError(string SourceFile, string ImportPath);
