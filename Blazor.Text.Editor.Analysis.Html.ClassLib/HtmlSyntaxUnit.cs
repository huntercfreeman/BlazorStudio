using Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxItems;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class HtmlSyntaxUnit
{
    public HtmlSyntaxUnit(
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
        public TagSyntax RootTagSyntax { get; set; }
        public TextEditorHtmlDiagnosticBag TextEditorHtmlDiagnosticBag { get; set; }

        public HtmlSyntaxUnit Build()
        {
            return new HtmlSyntaxUnit(
                RootTagSyntax,
                TextEditorHtmlDiagnosticBag);
        }
    }
}