using System.Collections.Immutable;
using Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class TextEditorHtmlLexer : ILexer
{
    public async Task<ImmutableArray<TextEditorTextSpan>> Lex(string content)
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(content);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);
        
        List<TextEditorTextSpan> textEditorTextSpans = new();
        
        // Tag Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.TagNameSyntaxes
                .Select(tns => tns.TextEditorTextSpan));
        }
        
        // InjectedLanguageFragmentSyntaxes
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.InjectedLanguageFragmentSyntaxes
                .Select(ilfs => ilfs.TextEditorTextSpan));
        }

        return textEditorTextSpans.ToImmutableArray();
    }
}