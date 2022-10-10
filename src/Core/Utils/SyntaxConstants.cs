using System.Collections.Immutable;
using Loretta.CodeAnalysis.Lua;
using Loretta.CodeAnalysis.Lua.Syntax;

namespace LuaPack.Core;

internal static class SyntaxConstants
{
    public const string ImportFunctionName = "__packer_import",
                        ImportFileTableName = "__packer_import_file_funcs";

    public static readonly IdentifierNameSyntax ImportFunctionNameSyntax = SyntaxFactory.IdentifierName(ImportFunctionName);

    public static readonly ImmutableArray<StatementSyntax> NonCachingHeader = ImmutableArray.Create(
        SyntaxFactory.ParseStatement(
            $$"""
            local {{ImportFileTableName}} = {}
            """),
        SyntaxFactory.ParseStatement(
            $"""
            local function {ImportFunctionName}(name, ...)
                local file = assert({ImportFileTableName}[name], "file not found")
                return file(...)
            end
            """));

    private const string ImportCacheName = "__packer_import_cache",
                         ImportNilCacheValueName = "__packer_import_cache_nil_value";

    public static readonly ImmutableArray<StatementSyntax> CachingHeader = ImmutableArray.Create(
        SyntaxFactory.ParseStatement(
            $$"""
            local {{ImportFileTableName}}, {{ImportFunctionName}} = {}
            """),
        SyntaxFactory.ParseStatement(
            $$"""
            do
                local {{ImportCacheName}}, {{ImportNilCacheValueName}} = {}, {}
                function {{ImportFunctionName}}(name, ...)
                    local cached = {{ImportCacheName}}[name]

                    if cached == nil then
                        local file = assert({{ImportFileTableName}}[name], "file not found")
                        cached = file(...)
                        if cached == nil then
                            cached = {{ImportNilCacheValueName}}
                        end
                        {{ImportCacheName}}[name] = cached
                    end

                    if cached == {{ImportNilCacheValueName}} then
                        cached = nil
                    end

                    return cached
                end
            end
            """));
}
