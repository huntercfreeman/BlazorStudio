namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class IdentifierToken : ISyntaxToken
{
    public IdentifierToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.IdentifierToken;
}