namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class OpenBraceToken : ISyntaxToken
{
    public OpenBraceToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenBraceToken;
}
