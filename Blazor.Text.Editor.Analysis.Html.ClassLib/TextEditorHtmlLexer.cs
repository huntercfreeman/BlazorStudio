using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class TextEditorHtmlLexer : ILexer
{
    public async Task<ImmutableArray<TextEditorTextSpan>> Lex(string content)
    {
        var syntaxTree = HtmlSyntaxTree.ParseText(content);

        var syntaxNodeRoot = syntaxTree.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);
        
        List<TextEditorTextSpan> textEditorTextSpans = new();
        
        // Tag Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.TagNameSyntaxes
                .Select(tns => tns.TextEditorTextSpan));
        }

        return textEditorTextSpans.ToImmutableArray();
    }
}