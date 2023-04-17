namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public interface ISyntaxToken
{
    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind { get; }
}