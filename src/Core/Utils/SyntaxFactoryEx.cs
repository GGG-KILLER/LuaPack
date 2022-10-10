using Loretta.CodeAnalysis.Lua;
using Loretta.CodeAnalysis.Lua.Syntax;

namespace LuaPack.Core;

internal static class SyntaxFactoryEx
{
    public static LiteralExpressionSyntax StringExpression(string value) =>
        SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));

    public static StringFunctionArgumentSyntax StringArgument(string value) =>
        SyntaxFactory.StringFunctionArgument(StringExpression(value));
}
