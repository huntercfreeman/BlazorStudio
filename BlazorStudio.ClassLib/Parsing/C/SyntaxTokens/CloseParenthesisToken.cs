namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class CloseParenthesisToken : ISyntaxToken
{
    public CloseParenthesisToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseParenthesisToken;
}
