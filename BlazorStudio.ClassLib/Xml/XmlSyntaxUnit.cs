using BlazorStudio.ClassLib.Xml.SyntaxObjects;

namespace BlazorStudio.ClassLib.Xml;

public class XmlSyntaxUnit
{
    public XmlSyntaxUnit(
        TagSyntax rootTagSyntax,
        XmlDiagnosticBag xmlDiagnosticBag)
    {
        XmlDiagnosticBag = xmlDiagnosticBag;
        RootTagSyntax = rootTagSyntax;
    }

    public TagSyntax RootTagSyntax { get; }
    public XmlDiagnosticBag XmlDiagnosticBag { get; }

    public class XmlSyntaxUnitBuilder
    {
        public XmlSyntaxUnitBuilder(TagSyntax rootTagSyntax, XmlDiagnosticBag textEditorHtmlDiagnosticBag)
        {
            RootTagSyntax = rootTagSyntax;
            XmlDiagnosticBag = textEditorHtmlDiagnosticBag;
        }

        public TagSyntax RootTagSyntax { get; }
        public XmlDiagnosticBag XmlDiagnosticBag { get; }

        public XmlSyntaxUnit Build()
        {
            return new XmlSyntaxUnit(
                RootTagSyntax,
                XmlDiagnosticBag);
        }
    }
}