using BlazorTextEditor.RazorLib.Analysis;
using System.Collections;

namespace BlazorStudio.ClassLib.Parsing;

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

    public void Report(BlazorStudioDiagnosticLevel blazorStudioDiagnosticLevel,
        string message,
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        _blazorStudioDiagnostics.Add(
            new BlazorStudioDiagnostic(
                blazorStudioDiagnosticLevel,
                message,
                blazorStudioTextSpan));
    }

    public void ReportEndOfFileUnexpected(BlazorStudioTextSpan blazorStudioTextSpan)
    {
        Report(
            BlazorStudioDiagnosticLevel.Error,
            "'End of file' was unexpected.",
            blazorStudioTextSpan);
    }

    public void ReportUnexpectedToken(
        BlazorStudioTextSpan blazorStudioTextSpan,
        string unexpectedToken)
    {
        Report(
            BlazorStudioDiagnosticLevel.Error,
            $"Unexpected token: '{unexpectedToken}'",
            blazorStudioTextSpan);
    }
}