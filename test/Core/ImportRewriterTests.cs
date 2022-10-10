using Loretta.CodeAnalysis.Lua;
using Loretta.CodeAnalysis.Text;

namespace LuaPack.Core.Tests;

public class ImportRewriterTests
{
    [Theory]
    [InlineData(
        "/test/x/something.lua",
        """
        local x = dofile './wanted.lua'
        """,
        """
        local x = __packer_import "x/wanted.lua"
        """
    )]
    [InlineData(
        "/test/x/something.lua",
        """
        local x = dofile '../wanted.lua'
        """,
        """
        local x = __packer_import "wanted.lua"
        """
    )]
    public void ImportRewriter_CorrectlyRewritesRequiresToBeRelativeToTheProjectRoot(string sourcePath, string sourceText, string expectedText)
    {
        const string root = "/test/";

        // Given
        var source = SourceText.From(sourceText);
        var tree = LuaSyntaxTree.ParseText(source, new LuaParseOptions(LuaSyntaxOptions.All), sourcePath);

        // When
        var rewritten = ImportRewriter.Rewrite(root, tree, out _, out var diagnostics);

        // Then
        Assert.Empty(diagnostics);
        Assert.Equal(expectedText, rewritten.ToFullString());
    }

    [Fact]
    public void ImportRewriter_EmitsErrorsWhenAnImportGoesOutsideOfTheProjectRoot()
    {
        // Given
        const string root = "/test/";
        var source = SourceText.From("""
                                                 local x = dofile '../wanted.lua'
                                                 """);
        var tree = LuaSyntaxTree.ParseText(
            source,
            new LuaParseOptions(LuaSyntaxOptions.All),
            Path.Combine(root, "main.lua"));

        // When
        var rewritten = ImportRewriter.Rewrite(root, tree, out _, out var diagnostics);

        // Then
        var diag = Assert.Single(diagnostics);
        Assert.Equal("/test/main.lua(1,11): error PACK0001: The file being imported through path '../wanted.lua' is outside of the workspace. If you wish to import that file, move the workspace file to a folder that you can access it from", diag.ToString());

        Assert.Equal(
            """
            local x = dofile '../wanted.lua'
            """,
            rewritten.ToFullString()
        );
    }
}
