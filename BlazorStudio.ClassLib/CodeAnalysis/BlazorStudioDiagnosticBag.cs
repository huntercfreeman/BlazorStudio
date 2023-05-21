using BlazorTextEditor.RazorLib.Lexing;
using System.Collections;

namespace BlazorStudio.ClassLib.CodeAnalysis;

public class BlazorStudioDiagnosticBag : IEnumerable<BlazorStudioDiagnostic>
{
    private readonly List<BlazorStudioDiagnostic> _blazorStudioDiagnostics = new();

    public IEnumerator<BlazorStudioDiagnostic> GetEnumerator()
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
            BlazorStudioDiagnosticLevel.Error,
            "'End of file' was unexpected.",
            textEditorTextSpan);
    }

    public void ReportUnexpectedToken(
        TextEditorTextSpan textEditorTextSpan,
        string unexpectedToken)
    {
        Report(
            BlazorStudioDiagnosticLevel.Error,
            $"Unexpected token: '{unexpectedToken}'",
            textEditorTextSpan);
    }

    public void ReportUndefindFunction(
        TextEditorTextSpan textEditorTextSpan,
        string undefinedFunctionName)
    {
        Report(
            BlazorStudioDiagnosticLevel.Error,
            $"Undefined function: '{undefinedFunctionName}'",
            textEditorTextSpan);
    }

    public void ReportReturnStatementsAreStillBeingImplemented(
        TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            BlazorStudioDiagnosticLevel.Hint,
            $"Parsing of return statements is still being implemented.",
            textEditorTextSpan);
    }

    private void Report(
        BlazorStudioDiagnosticLevel blazorStudioDiagnosticLevel,
        string message,
        TextEditorTextSpan textEditorTextSpan)
    {
        _blazorStudioDiagnostics.Add(
            new BlazorStudioDiagnostic(
                blazorStudioDiagnosticLevel,
                message,
                textEditorTextSpan));
    }
}