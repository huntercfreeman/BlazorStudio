using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
}