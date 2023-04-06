using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Xml.SyntaxActors;

public class XmlLexer
{
    public Task<ImmutableArray<TextEditorTextSpan>> Lex(string text)
    {
        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(text);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var htmlSyntaxWalker = new HtmlSyntaxWalker();

        htmlSyntaxWalker.Visit(syntaxNodeRoot);

        List<TextEditorTextSpan> textEditorTextSpans = new();

        // Tag Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.TagNameSyntaxes
                .Select(tn => tn.TextEditorTextSpan));
        }

        // InjectedLanguageFragmentSyntaxes
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.InjectedLanguageFragmentSyntaxes
                .Select(ilf => ilf.TextEditorTextSpan));
        }
        
        // Attribute Names
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeNameSyntaxes
                .Select(an => an.TextEditorTextSpan));
        }
        
        // Attribute Values
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.AttributeValueSyntaxes
                .Select(av => av.TextEditorTextSpan));
        }
        
        // Comments
        {
            textEditorTextSpans.AddRange(htmlSyntaxWalker.CommentSyntaxes
                .Select(c => c.TextEditorTextSpan));
        }

        return Task.FromResult(textEditorTextSpans.ToImmutableArray());
    }
}