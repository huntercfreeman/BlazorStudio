using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;

namespace BlazorStudio.ClassLib.Xml;

public interface IXmlSyntax
{
    public XmlSyntaxKind XmlSyntaxKind { get; }
    public ImmutableArray<IXmlSyntax> ChildXmlSyntaxes { get; }
}