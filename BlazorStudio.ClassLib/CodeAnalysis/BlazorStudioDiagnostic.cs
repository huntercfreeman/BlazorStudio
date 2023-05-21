using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.CodeAnalysis;

public record BlazorStudioDiagnostic(
    BlazorStudioDiagnosticLevel BlazorStudioDiagnosticLevel,
    string Message,
    TextEditorTextSpan TextEditorTextSpan);