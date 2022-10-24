using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public record TextEditorDiagnostic(
    DiagnosticLevel DiagnosticLevel,
    string Message,
    TextEditorTextSpan TextEditorTextSpan);