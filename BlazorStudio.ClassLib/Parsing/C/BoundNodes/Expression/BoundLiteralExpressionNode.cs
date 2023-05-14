using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;

public class BoundLiteralExpressionNode : IBoundExpressionNode
{
    public BoundLiteralExpressionNode(
        ISyntaxToken literalSyntaxToken,
        Type resultType)
    {
        ResultType = resultType;

        Children = new ISyntax[]
        {
            literalSyntaxToken
        }.ToImmutableArray();
    }

    public Type ResultType { get; }
    public ImmutableArray<ISyntax> Children { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.BoundLiteralExpressionNode;
}
