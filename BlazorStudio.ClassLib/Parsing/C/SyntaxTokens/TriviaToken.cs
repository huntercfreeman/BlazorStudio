namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class TriviaToken : ISyntaxToken
{
    public TriviaToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.TriviaToken;
}