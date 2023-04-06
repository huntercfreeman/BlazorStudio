using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;

namespace BlazorStudio.ClassLib.Xml;

public interface IXmlSyntax
{
    public HtmlSyntaxKind HtmlSyntaxKind { get; }
    public ImmutableArray<IXmlSyntax> ChildHtmlSyntaxes { get; }
}