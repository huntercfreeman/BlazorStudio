using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class TagNameSyntax : IXmlSyntax
{
    public TagNameSyntax(
        string value,
        TextEditorTextSpan textEditorTextSpan)
    {
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public string Value { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public XmlSyntaxKind XmlSyntaxKind => XmlSyntaxKind.TagName;
    public ImmutableArray<IXmlSyntax> ChildXmlSyntaxes { get; } = ImmutableArray<IXmlSyntax>.Empty;
}