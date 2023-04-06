using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class AttributeValueSyntax : IXmlSyntax
{
    public AttributeValueSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public XmlSyntaxKind XmlSyntaxKind => XmlSyntaxKind.AttributeValue;
    public ImmutableArray<IXmlSyntax> ChildXmlSyntaxes { get; } = ImmutableArray<IXmlSyntax>.Empty;
}