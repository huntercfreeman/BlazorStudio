using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class LibraryReferenceNode : ISyntaxNode
{
    public LibraryReferenceNode(
        LibraryReferenceToken libraryReferenceToken)
    {
        LibraryReferenceToken = libraryReferenceToken;
    }

    public LibraryReferenceToken LibraryReferenceToken { get; }
    
    public ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        LibraryReferenceToken
    }.ToImmutableArray();
    
    public SyntaxKind SyntaxKind => SyntaxKind.LibraryReferenceNode;
}