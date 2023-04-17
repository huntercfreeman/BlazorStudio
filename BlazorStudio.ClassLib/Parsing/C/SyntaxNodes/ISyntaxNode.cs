using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> Children { get; }
}