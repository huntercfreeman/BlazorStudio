using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public class InjectedLanguageFragmentSyntax : TagSyntax
{
    public InjectedLanguageFragmentSyntax(
        ImmutableArray<TagSyntax> childTagSyntaxes,
        string value,
        TextEditorTextSpan textEditorTextSpan,
        bool hasSpecialHtmlCharacter = false) 
        : base(
            null, 
            null,
            ImmutableArray<AttributeTupleSyntax>.Empty, 
            childTagSyntaxes, 
            TagKind.InjectedLanguageCodeBlock,
            hasSpecialHtmlCharacter)
    {
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public string Value { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
}