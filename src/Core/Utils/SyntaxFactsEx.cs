using System.Diagnostics.CodeAnalysis;
using Loretta.CodeAnalysis.Lua;
using Loretta.CodeAnalysis.Lua.Syntax;

namespace LuaPack.Core;

internal static class SyntaxFactsEx
{
    public static bool IsImportName(ExpressionSyntax expression) =>
        expression is IdentifierNameSyntax { Name: "dofile" };

    public static bool IsStringLiteral(ExpressionSyntax expression)
    {
        return expression is LiteralExpressionSyntax
        {
            RawKind: (int)SyntaxKind.StringLiteralExpression
        };
    }

    public static bool IsSingleStringArgument(
        FunctionArgumentSyntax functionArgument,
        [NotNullWhen(true)] out string? value)
    {
        if (functionArgument is StringFunctionArgumentSyntax stringFunctionArgument)
        {
            value = (string)stringFunctionArgument.Expression.Token.Value!;
            return true;
        }

        if (functionArgument is ExpressionListFunctionArgumentSyntax expressionListFunctionArgument
            && expressionListFunctionArgument.Expressions.Count == 1
            && IsStringLiteral(expressionListFunctionArgument.Expressions[0]))
        {
            value = (string)((LiteralExpressionSyntax)expressionListFunctionArgument.Expressions[0]).Token.Value!;
            return true;
        }

        value = null;
        return false;
    }

    public static bool IsImport(
        FunctionCallExpressionSyntax functionCall,
        [NotNullWhen(true)] out string? importedFile)
    {
        importedFile = null;
        return IsImportName(functionCall.Expression)
            && IsSingleStringArgument(functionCall.Argument, out importedFile);
    }
}
