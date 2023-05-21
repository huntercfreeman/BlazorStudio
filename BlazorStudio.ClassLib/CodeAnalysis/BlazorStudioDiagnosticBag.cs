using BlazorTextEditor.RazorLib.Analysis;
using BlazorTextEditor.RazorLib.Lexing;
using System.Collections;

namespace BlazorStudio.ClassLib.CodeAnalysis;

public class BlazorStudioDiagnosticBag : IEnumerable<TextEditorDiagnostic>
{
    private readonly List<TextEditorDiagnostic> _blazorStudioDiagnostics = new();

    public IEnumerator<TextEditorDiagnostic> GetEnumerator()
    {
        return _blazorStudioDiagnostics.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void ReportEndOfFileUnexpected(
        TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            "'End of file' was unexpected.",
            textEditorTextSpan);
    }

    public void ReportUnexpectedToken(
        TextEditorTextSpan textEditorTextSpan,
        string unexpectedToken)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Unexpected token: '{unexpectedToken}'",
            textEditorTextSpan);
    }

    public void ReportUndefindFunction(
        TextEditorTextSpan textEditorTextSpan,
        string undefinedFunctionName)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Undefined function: '{undefinedFunctionName}'",
            textEditorTextSpan);
    }

    public void ReportReturnStatementsAreStillBeingImplemented(
        TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Hint,
            $"Parsing of return statements is still being implemented.",
            textEditorTextSpan);
    }

    private void Report(
        TextEditorDiagnosticLevel blazorStudioDiagnosticLevel,
        string message,
        TextEditorTextSpan textEditorTextSpan)
    {
        _blazorStudioDiagnostics.Add(
            new TextEditorDiagnostic(
                blazorStudioDiagnosticLevel,
                message,
                textEditorTextSpan));
    }
}