using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> Children { get; }
}