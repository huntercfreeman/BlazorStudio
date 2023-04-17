namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class PlusToken : ISyntaxToken
{
    public PlusToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PlusToken;
}