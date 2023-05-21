using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes.Expression;

namespace BlazorStudio.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Expression;

public interface IBoundExpressionNode : IExpressionNode
{
    public Type ResultType { get; }
}
