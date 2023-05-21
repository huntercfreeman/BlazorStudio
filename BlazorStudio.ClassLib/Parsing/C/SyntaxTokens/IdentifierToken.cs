using BlazorTextEditor.RazorLib.Lexing;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class IdentifierToken : ISyntaxToken
{
    public IdentifierToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.IdentifierToken;
}