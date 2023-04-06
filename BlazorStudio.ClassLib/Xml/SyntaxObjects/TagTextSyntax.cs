using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class TagTextSyntax : TagSyntax, IXmlSyntax
{
    public TagTextSyntax(
        ImmutableArray<AttributeSyntax> attributeSyntaxes,
        ImmutableArray<IXmlSyntax> childXmlSyntaxes,
        string value,
        bool hasSpecialXmlCharacter = false)
        : base(
            null,
            null,
            attributeSyntaxes,
            childXmlSyntaxes,
            TagKind.Text,
            hasSpecialXmlCharacter)
    {
        Value = value;
    }

    public string Value { get; }

    public override XmlSyntaxKind XmlSyntaxKind => XmlSyntaxKind.TagText;
}