using Blazor.Text.Editor.Analysis.Html.ClassLib;

namespace Blazor.Text.Editor.Analysis.Razor.Tests;

public class LexerTests
{
    [Fact]
    public async Task Test1()
    {
        var content = @"some text";

        /*
         * Expected:
         *     -TextNode = 'some text'
         */

        var lexer = new TextEditorHtmlLexer();

        var textEditorTextSpans =
            await lexer.Lex(content);
    }
}