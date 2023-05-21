using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class CloseParenthesisToken : ISyntaxToken
{
    public CloseParenthesisToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseParenthesisToken;
}
