using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
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
    
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.AttributeValue;
    public ImmutableArray<IXmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IXmlSyntax>.Empty;
}