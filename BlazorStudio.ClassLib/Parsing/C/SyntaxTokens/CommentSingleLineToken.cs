namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class CommentSingleLineToken : ISyntaxToken
{
    public CommentSingleLineToken(
        BlazorStudioTextSpan blazorStudioTextSpan)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CommentSingleLineToken;
}