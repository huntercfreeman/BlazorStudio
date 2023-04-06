using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;
using TagKind = BlazorStudio.ClassLib.Xml.SyntaxEnums.TagKind;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class InjectedLanguageFragmentSyntax : TagSyntax, IXmlSyntax
{
    public InjectedLanguageFragmentSyntax(
        ImmutableArray<IXmlSyntax> childHtmlSyntaxes,
        string value,
        TextEditorTextSpan textEditorTextSpan,
        bool hasSpecialHtmlCharacter = false)
        : base(
            null,
            null,
            ImmutableArray<AttributeSyntax>.Empty,
            childHtmlSyntaxes,
            TagKind.InjectedLanguageCodeBlock,
            hasSpecialHtmlCharacter)
    {
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public string Value { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public override HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.InjectedLanguageFragment;
}