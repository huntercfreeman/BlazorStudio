using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;

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
    
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.Attribute;
    public ImmutableArray<IXmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IXmlSyntax>.Empty;
}