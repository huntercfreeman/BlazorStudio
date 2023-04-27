namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class StatementDelimiterToken : ISyntaxToken
{
    public StatementDelimiterToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StatementDelimiterToken;
}