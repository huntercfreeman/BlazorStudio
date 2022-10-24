using System.Collections;
using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class TextEditorDiagnosticBag : IEnumerable<TextEditorDiagnostic>
{
    private readonly List<TextEditorDiagnostic> _textEditorDiagnostics = new();

    public void Report(DiagnosticLevel diagnosticLevel,
        string message,
        TextEditorTextSpan textEditorTextSpan)
    {
        _textEditorDiagnostics.Add(
            new TextEditorDiagnostic(
                diagnosticLevel,
                message,
                textEditorTextSpan));
    }
    
    public void ReportEndOfFileUnexpected(TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            DiagnosticLevel.Error,
            $"'End of file' was unexpected." +
            $" Wanted an ['attribute' OR 'closing tag'].",
            textEditorTextSpan);
    }
    
    public IEnumerator<TextEditorDiagnostic> GetEnumerator()
    {
        return _textEditorDiagnostics.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}