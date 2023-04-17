namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class StringLiteralToken : ISyntaxToken
{
    public StringLiteralToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringLiteralToken;
}