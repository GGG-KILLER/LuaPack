using System.Text.Json.Serialization;
using Loretta.CodeAnalysis.Lua;

namespace LuaPack.Core;

public sealed class ProjectFile
{
    [JsonConstructor]
    public ProjectFile(
        string preset,
        string entryPoint,
        string outputFile,
        IEnumerable<string> files,
        bool cachedIncludes = false,
        NullableLuaSyntaxOptions? presetOverrides = null)
    {
        Files = files ?? throw new ArgumentNullException(nameof(files));
        Preset = preset ?? throw new ArgumentNullException(nameof(preset));
        EntryPoint = entryPoint ?? throw new ArgumentNullException(nameof(entryPoint));
        OutputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
        CachedIncludes = cachedIncludes;
        PresetOverrides = presetOverrides;
    }

    public string Preset { get; }

    public string EntryPoint { get; }

    public string OutputFile { get; }

    public IEnumerable<string> Files { get; }

    public bool CachedIncludes { get; }

    public NullableLuaSyntaxOptions? PresetOverrides { get; }

    public LuaParseOptions GetParseOptions()
    {
        LuaSyntaxOptions syntaxOptions = Preset switch
        {
            nameof(LuaSyntaxOptions.Lua51) => LuaSyntaxOptions.Lua51,
            nameof(LuaSyntaxOptions.Lua52) => LuaSyntaxOptions.Lua52,
            nameof(LuaSyntaxOptions.Lua53) => LuaSyntaxOptions.Lua53,
            nameof(LuaSyntaxOptions.Lua54) => LuaSyntaxOptions.Lua54,
            nameof(LuaSyntaxOptions.LuaJIT20) => LuaSyntaxOptions.LuaJIT20,
            nameof(LuaSyntaxOptions.LuaJIT21) => LuaSyntaxOptions.LuaJIT21,
            nameof(LuaSyntaxOptions.GMod) => LuaSyntaxOptions.GMod,
            nameof(LuaSyntaxOptions.FiveM) => LuaSyntaxOptions.FiveM,
            nameof(LuaSyntaxOptions.Roblox) => LuaSyntaxOptions.Luau,
            nameof(LuaSyntaxOptions.Luau) => LuaSyntaxOptions.Luau,
            nameof(LuaSyntaxOptions.All) => LuaSyntaxOptions.All,
            nameof(LuaSyntaxOptions.AllWithIntegers) => LuaSyntaxOptions.AllWithIntegers,
            _ => throw new InvalidOperationException($"Preset '{Preset}' is not a valid preset.")
        };

        syntaxOptions = syntaxOptions.With(
            PresetOverrides?.AcceptBinaryNumbers ?? syntaxOptions.AcceptBinaryNumbers,
            PresetOverrides?.AcceptCCommentSyntax ?? syntaxOptions.AcceptCCommentSyntax,
            PresetOverrides?.AcceptCompoundAssignment ?? syntaxOptions.AcceptCompoundAssignment,
            PresetOverrides?.AcceptEmptyStatements ?? syntaxOptions.AcceptEmptyStatements,
            PresetOverrides?.AcceptCBooleanOperators ?? syntaxOptions.AcceptCBooleanOperators,
            PresetOverrides?.AcceptGoto ?? syntaxOptions.AcceptGoto,
            PresetOverrides?.AcceptHexEscapesInStrings ?? syntaxOptions.AcceptHexEscapesInStrings,
            PresetOverrides?.AcceptHexFloatLiterals ?? syntaxOptions.AcceptHexFloatLiterals,
            PresetOverrides?.AcceptOctalNumbers ?? syntaxOptions.AcceptOctalNumbers,
            PresetOverrides?.AcceptShebang ?? syntaxOptions.AcceptShebang,
            PresetOverrides?.AcceptUnderscoreInNumberLiterals ?? syntaxOptions.AcceptUnderscoreInNumberLiterals,
            PresetOverrides?.UseLuaJitIdentifierRules ?? syntaxOptions.UseLuaJitIdentifierRules,
            PresetOverrides?.AcceptBitwiseOperators ?? syntaxOptions.AcceptBitwiseOperators,
            PresetOverrides?.AcceptWhitespaceEscape ?? syntaxOptions.AcceptWhitespaceEscape,
            PresetOverrides?.AcceptUnicodeEscape ?? syntaxOptions.AcceptUnicodeEscape,
            PresetOverrides?.ContinueType ?? syntaxOptions.ContinueType,
            PresetOverrides?.AcceptIfExpressions ?? syntaxOptions.AcceptIfExpressions,
            PresetOverrides?.AcceptHashStrings ?? syntaxOptions.AcceptHashStrings,
            PresetOverrides?.AcceptInvalidEscapes ?? syntaxOptions.AcceptInvalidEscapes,
            PresetOverrides?.AcceptLocalVariableAttributes ?? syntaxOptions.AcceptLocalVariableAttributes,
            PresetOverrides?.BinaryIntegerFormat ?? syntaxOptions.BinaryIntegerFormat,
            PresetOverrides?.OctalIntegerFormat ?? syntaxOptions.OctalIntegerFormat,
            PresetOverrides?.DecimalIntegerFormat ?? syntaxOptions.DecimalIntegerFormat,
            PresetOverrides?.HexIntegerFormat ?? syntaxOptions.HexIntegerFormat);

        return new LuaParseOptions(syntaxOptions);
    }
}
