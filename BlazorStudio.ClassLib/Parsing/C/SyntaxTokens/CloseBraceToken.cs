namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class CloseBraceToken : ISyntaxToken
{
    public CloseBraceToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseBraceToken;
}
