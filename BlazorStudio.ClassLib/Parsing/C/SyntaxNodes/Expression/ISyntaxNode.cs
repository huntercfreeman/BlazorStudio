using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> Children { get; }
}
