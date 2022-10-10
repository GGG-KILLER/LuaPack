using System.Text.Json.Serialization;
using Loretta.CodeAnalysis.Lua;

namespace LuaPack.Core;

public class NullableLuaSyntaxOptions
{
    [JsonConstructor]
    public NullableLuaSyntaxOptions(
        bool? acceptBinaryNumbers = null,
        bool? acceptCCommentSyntax = null,
        bool? acceptCompoundAssignment = null,
        bool? acceptEmptyStatements = null,
        bool? acceptCBooleanOperators = null,
        bool? acceptGoto = null,
        bool? acceptHexEscapesInStrings = null,
        bool? acceptHexFloatLiterals = null,
        bool? acceptOctalNumbers = null,
        bool? acceptShebang = null,
        bool? acceptUnderscoreInNumberLiterals = null,
        bool? useLuaJitIdentifierRules = null,
        bool? acceptBitwiseOperators = null,
        bool? acceptWhitespaceEscape = null,
        bool? acceptUnicodeEscape = null,
        ContinueType? continueType = null,
        bool? acceptIfExpressions = null,
        bool? acceptHashStrings = null,
        bool? acceptInvalidEscapes = null,
        bool? acceptLocalVariableAttributes = null,
        IntegerFormats? binaryIntegerFormat = null,
        IntegerFormats? octalIntegerFormat = null,
        IntegerFormats? decimalIntegerFormat = null,
        IntegerFormats? hexIntegerFormat = null,
        bool? acceptTypedLua = null,
        bool? acceptFloorDivision = null,
        bool? acceptLuaJITNumberSuffixes = null)
    {
        AcceptBinaryNumbers = acceptBinaryNumbers;
        AcceptCCommentSyntax = acceptCCommentSyntax;
        AcceptCompoundAssignment = acceptCompoundAssignment;
        AcceptEmptyStatements = acceptEmptyStatements;
        AcceptCBooleanOperators = acceptCBooleanOperators;
        AcceptGoto = acceptGoto;
        AcceptHexEscapesInStrings = acceptHexEscapesInStrings;
        AcceptHexFloatLiterals = acceptHexFloatLiterals;
        AcceptOctalNumbers = acceptOctalNumbers;
        AcceptShebang = acceptShebang;
        AcceptUnderscoreInNumberLiterals = acceptUnderscoreInNumberLiterals;
        UseLuaJitIdentifierRules = useLuaJitIdentifierRules;
        AcceptBitwiseOperators = acceptBitwiseOperators;
        AcceptWhitespaceEscape = acceptWhitespaceEscape;
        AcceptUnicodeEscape = acceptUnicodeEscape;
        ContinueType = continueType;
        AcceptIfExpressions = acceptIfExpressions;
        AcceptHashStrings = acceptHashStrings;
        AcceptInvalidEscapes = acceptInvalidEscapes;
        AcceptLocalVariableAttributes = acceptLocalVariableAttributes;
        BinaryIntegerFormat = binaryIntegerFormat;
        OctalIntegerFormat = octalIntegerFormat;
        DecimalIntegerFormat = decimalIntegerFormat;
        HexIntegerFormat = hexIntegerFormat;
        AcceptTypedLua = acceptTypedLua;
        AcceptFloorDivision = acceptFloorDivision;
        AcceptLuaJITNumberSuffixes = acceptLuaJITNumberSuffixes;
    }

    public bool? AcceptBinaryNumbers { get; }
    public bool? AcceptCCommentSyntax { get; }
    public bool? AcceptCompoundAssignment { get; }
    public bool? AcceptEmptyStatements { get; }
    public bool? AcceptCBooleanOperators { get; }
    public bool? AcceptGoto { get; }
    public bool? AcceptHexEscapesInStrings { get; }
    public bool? AcceptHexFloatLiterals { get; }
    public bool? AcceptOctalNumbers { get; }
    public bool? AcceptShebang { get; }
    public bool? AcceptUnderscoreInNumberLiterals { get; }
    public bool? UseLuaJitIdentifierRules { get; }
    public bool? AcceptBitwiseOperators { get; }
    public bool? AcceptWhitespaceEscape { get; }
    public bool? AcceptUnicodeEscape { get; }
    public ContinueType? ContinueType { get; }
    public bool? AcceptIfExpressions { get; }
    public bool? AcceptHashStrings { get; }
    public bool? AcceptInvalidEscapes { get; }
    public bool? AcceptLocalVariableAttributes { get; }
    public IntegerFormats? BinaryIntegerFormat { get; }
    public IntegerFormats? OctalIntegerFormat { get; }
    public IntegerFormats? DecimalIntegerFormat { get; }
    public IntegerFormats? HexIntegerFormat { get; }
    public bool? AcceptTypedLua { get; }
    public bool? AcceptFloorDivision { get; }
    public bool? AcceptLuaJITNumberSuffixes { get; }
}
