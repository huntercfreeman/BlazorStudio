namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class EqualsToken : ISyntaxToken
{
    public EqualsToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.EqualsToken;
}