using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class AttributeNameSyntax : IXmlSyntax
{
    public AttributeNameSyntax(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public XmlSyntaxKind XmlSyntaxKind => XmlSyntaxKind.AttributeName;
    public ImmutableArray<IXmlSyntax> ChildXmlSyntaxes { get; } = ImmutableArray<IXmlSyntax>.Empty;
}