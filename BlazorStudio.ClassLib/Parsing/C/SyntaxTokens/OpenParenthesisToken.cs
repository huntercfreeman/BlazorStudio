namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class OpenParenthesisToken : ISyntaxToken
{
    public OpenParenthesisToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenParenthesisToken;
}