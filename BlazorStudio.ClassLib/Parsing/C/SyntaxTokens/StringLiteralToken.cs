using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class StringLiteralToken : ISyntaxToken
{
    public StringLiteralToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringLiteralToken;
}