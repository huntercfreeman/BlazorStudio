namespace BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

public class LibraryReferenceToken : ISyntaxToken
{
    public LibraryReferenceToken(
        BlazorStudioTextSpan blazorStudioTextSpan,
        bool isAbsolutePath)
    {
        BlazorStudioTextSpan = blazorStudioTextSpan;
        IsAbsolutePath = isAbsolutePath;
    }

    public BlazorStudioTextSpan BlazorStudioTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.LibraryReferenceToken;
    public bool IsAbsolutePath { get; }
}