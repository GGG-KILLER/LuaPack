using System.Text;
using Loretta.CodeAnalysis;
using Loretta.CodeAnalysis.Text;

namespace LuaPack;

internal readonly struct DiagnosticWriter
{
    private readonly StringBuilder _builder;
    private readonly TextWriter _out;
    private readonly int _maxLineDigits;

    private DiagnosticWriter(TextWriter @out, int maxLineDigits)
    {
        _builder = new();
        _out = @out;
        _maxLineDigits = maxLineDigits;
    }

    private async Task WriteDiagnostic(Diagnostic diagnostic, string root)
    {
        _builder.Clear()
                .Append(PathHelper.MakePathRelative(root, diagnostic.Location.SourceTree!.FilePath))
                .Append('(')
                .Append(diagnostic.Location.GetLineSpan().StartLinePosition)
                .Append("):")
                .AppendLine()
                .Append(diagnostic.Severity.ToString().ToLowerInvariant())
                .Append(' ')
                .Append(diagnostic.Id)
                .Append(": ")
                .Append(diagnostic.GetMessage());
        await _out.WriteLineAsync(_builder);
    }

    private async Task WritePadding()
    {
        _builder.Clear()
                .Insert(0, " ", _maxLineDigits)
                .Append(" |");
        await _out.WriteLineAsync(_builder);
    }

    private async Task WriteSourceLine(TextLine line)
    {
        _builder.Clear()
                .Append((line.LineNumber + 1).ToString().PadLeft(_maxLineDigits, '0'))
                .Append(" | ")
                .Append(line.ToString());
        await _out.WriteLineAsync(_builder);
    }

    private async Task WriteHighlight(int start, int length)
    {
        _builder.Clear()
                .Insert(0, " ", _maxLineDigits)
                .Append(" | ")
                .Insert(_builder.Length, " ", start)
                .Insert(_builder.Length, "^", length);
        await _out.WriteLineAsync(_builder);
    }

    public static async Task WriteDiagnostic(TextWriter @out, string root, Diagnostic diagnostic)
    {
        var lineBuilder = new StringBuilder();
        var text = await diagnostic.Location.SourceTree!.GetTextAsync();
        var span = diagnostic.Location.GetLineSpan();

        var firstLine = text.Lines.Min(l => l.LineNumber);
        var lastLine = text.Lines.Max(l => l.LineNumber);
        var contextStartLine = Math.Max(firstLine, span.StartLinePosition.Line - 2);
        var contextEndLine = Math.Min(lastLine, span.EndLinePosition.Line + 2);

        var maxLineDigits = Math.Max(digitCount(contextStartLine + 1), digitCount(contextEndLine + 1));
        var writer = new DiagnosticWriter(@out, maxLineDigits);

        await writer.WriteDiagnostic(diagnostic, root);
        await writer.WritePadding();
        for (var lineIdx = contextStartLine; lineIdx <= contextEndLine; lineIdx++)
        {
            var line = text.Lines[lineIdx];
            await writer.WriteSourceLine(line);

            var startPos = 0;
            var endPos = line.End - line.Start;

            if (span.StartLinePosition.Line <= lineIdx && lineIdx <= span.StartLinePosition.Line)
            {
                if (span.StartLinePosition.Line == lineIdx)
                {
                    startPos = span.StartLinePosition.Character;
                }
                if (span.EndLinePosition.Line == lineIdx)
                {
                    endPos = span.EndLinePosition.Character;
                }

                await writer.WriteHighlight(startPos, Math.Max(1, endPos - startPos));
            }
        }
        await writer.WritePadding();

        static int digitCount(int n) =>
            n == 0 ? 1 : 1 + (int) Math.Floor(Math.Log10(n));
    }
}
