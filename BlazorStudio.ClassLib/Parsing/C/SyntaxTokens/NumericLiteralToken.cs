namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class NumericLiteralToken : ISyntaxToken
{
    public NumericLiteralToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
}