namespace BlazorStudio.ClassLib.Parsing;

public record BlazorStudioDiagnostic(
    BlazorStudioDiagnosticLevel BlazorStudioDiagnosticLevel,
    string Message,
    BlazorStudioTextSpan BlazorStudioTextSpan);