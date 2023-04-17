namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class KeywordToken : ISyntaxToken
{
    public KeywordToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.KeywordToken;
}