using BlazorTextEditor.RazorLib.Analysis;
using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Xml;

public class XmlDiagnosticBag : TextEditorDiagnosticBag
{
    public void ReportTagNameMissing(TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            DiagnosticLevel.Error,
            "Missing tag name.",
            textEditorTextSpan);
    }

    public void ReportOpenTagWithUnMatchedCloseTag(
        string openTagName,
        string closeTagName,
        TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            DiagnosticLevel.Error,
            $"Open tag: '{openTagName}' has an unmatched close tag: {closeTagName}.",
            textEditorTextSpan);
    }
}