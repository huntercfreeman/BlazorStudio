using BlazorTextEditor.RazorLib.Lexing;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public class TagNameSyntax
{
    public TagNameSyntax(
        string value, 
        TextEditorTextSpan textEditorTextSpan)
    {
        Value = value;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public string Value { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
}