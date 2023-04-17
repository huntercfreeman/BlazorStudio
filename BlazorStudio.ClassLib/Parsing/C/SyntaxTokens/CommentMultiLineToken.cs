namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class CommentMultiLineToken : ISyntaxToken
{
    public CommentMultiLineToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CommentMultiLineToken;
}