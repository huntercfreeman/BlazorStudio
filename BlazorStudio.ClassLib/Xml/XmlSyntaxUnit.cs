using BlazorStudio.ClassLib.Xml.SyntaxObjects;

namespace BlazorStudio.ClassLib.Xml;

public class XmlSyntaxUnit
{
    public XmlSyntaxUnit(
        TagSyntax rootTagSyntax,
        TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag)
    {
        TextEditorHtmlDiagnosticBag = textEditorHtmlDiagnosticBag;
        RootTagSyntax = rootTagSyntax;
    }

    public TagSyntax RootTagSyntax { get; }
    public TextEditorHtmlDiagnosticBag TextEditorHtmlDiagnosticBag { get; }

    public class HtmlSyntaxUnitBuilder
    {
        public HtmlSyntaxUnitBuilder(TagSyntax rootTagSyntax, TextEditorHtmlDiagnosticBag textEditorHtmlDiagnosticBag)
        {
            RootTagSyntax = rootTagSyntax;
            TextEditorHtmlDiagnosticBag = textEditorHtmlDiagnosticBag;
        }

        public TagSyntax RootTagSyntax { get; }
        public TextEditorHtmlDiagnosticBag TextEditorHtmlDiagnosticBag { get; }

        public XmlSyntaxUnit Build()
        {
            return new XmlSyntaxUnit(
                RootTagSyntax,
                TextEditorHtmlDiagnosticBag);
        }
    }
}