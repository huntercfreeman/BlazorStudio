namespace Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxItems;

public class AttributeTupleSyntax
{
    public AttributeTupleSyntax(
        AttributeNameSyntax attributeNameSyntax, 
        AttributeValueSyntax attributeValueSyntax)
    {
        AttributeNameSyntax = attributeNameSyntax;
        AttributeValueSyntax = attributeValueSyntax;
    }

    public AttributeNameSyntax AttributeNameSyntax { get; }
    public AttributeValueSyntax AttributeValueSyntax { get; }
}