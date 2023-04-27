namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class EndOfFileToken : ISyntaxToken
{
    public EndOfFileToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.EndOfFileToken;
}