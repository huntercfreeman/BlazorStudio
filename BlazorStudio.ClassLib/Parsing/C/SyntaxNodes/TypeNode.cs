using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.Symbols;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class TypeNode : ISyntaxNode
{
    public TypeNode(
        ISyntaxToken token,
        ITypeSymbol typeSymbol)
    {
        Token = token;
        TypeSymbol = typeSymbol;
    }
    
    public ISyntaxToken Token { get; }
    public ITypeSymbol TypeSymbol { get; }

    public ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        Token,
        TypeSymbol,
    }.ToImmutableArray();
    public SyntaxKind SyntaxKind => SyntaxKind.TypeNode;
}