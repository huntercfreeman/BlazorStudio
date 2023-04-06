using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using TagKind = BlazorStudio.ClassLib.Xml.SyntaxEnums.TagKind;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class TagTextSyntax : TagSyntax, IXmlSyntax
{
    public TagTextSyntax(
        ImmutableArray<AttributeSyntax> attributeSyntaxes,
        ImmutableArray<IXmlSyntax> childHtmlSyntaxes,
        string value,
        bool hasSpecialHtmlCharacter = false)
        : base(
            null,
            null,
            attributeSyntaxes,
            childHtmlSyntaxes,
            TagKind.Text,
            hasSpecialHtmlCharacter)
    {
        Value = value;
    }

    public string Value { get; }

    public override HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.TagText;
}