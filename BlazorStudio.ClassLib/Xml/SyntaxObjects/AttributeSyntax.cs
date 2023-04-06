using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class AttributeSyntax : IXmlSyntax
{
    public AttributeSyntax(
        AttributeNameSyntax attributeNameSyntax,
        AttributeValueSyntax attributeValueSyntax)
    {
        AttributeNameSyntax = attributeNameSyntax;
        AttributeValueSyntax = attributeValueSyntax;
    }

    public AttributeNameSyntax AttributeNameSyntax { get; }
    public AttributeValueSyntax AttributeValueSyntax { get; }
    
    public XmlSyntaxKind XmlSyntaxKind => XmlSyntaxKind.Attribute;
    public ImmutableArray<IXmlSyntax> ChildXmlSyntaxes { get; } = ImmutableArray<IXmlSyntax>.Empty;
}