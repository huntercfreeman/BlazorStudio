using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;

public interface IBoundExpressionNode : IExpressionNode
{
    public Type ResultType { get; }
}
