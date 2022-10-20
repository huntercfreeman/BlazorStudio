using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Shared;

public record TextEditorDiagnostic(
    DiagnosticLevel DiagnosticLevel,
    string Message,
    TextEditorTextSpan TextEditorTextSpan);