using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;

public interface IBoundExpressionNode : ISyntaxNode
{
    public Type ResultType { get; }
}
