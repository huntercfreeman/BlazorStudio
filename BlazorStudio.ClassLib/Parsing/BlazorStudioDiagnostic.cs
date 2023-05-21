using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Parsing;

public record BlazorStudioDiagnostic(
    BlazorStudioDiagnosticLevel BlazorStudioDiagnosticLevel,
    string Message,
    TextEditorTextSpan TextEditorTextSpan);