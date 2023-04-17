namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class PreprocessorDirectiveToken : ISyntaxToken
{
    public PreprocessorDirectiveToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorDirectiveToken;
}