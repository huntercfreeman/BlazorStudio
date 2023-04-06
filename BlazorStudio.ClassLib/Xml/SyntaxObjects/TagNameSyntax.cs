using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
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
    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.TagName;
    public ImmutableArray<IXmlSyntax> ChildHtmlSyntaxes { get; } = ImmutableArray<IXmlSyntax>.Empty;
}