namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public interface ISyntaxToken : ISyntax
{
    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
}