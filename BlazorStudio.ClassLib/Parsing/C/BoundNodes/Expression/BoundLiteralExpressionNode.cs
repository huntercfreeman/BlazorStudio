using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;

public class BoundLiteralExpressionNode : LiteralExpressionNode
{
    public BoundLiteralExpressionNode(
        ISyntaxToken literalSyntaxToken,
        Type type)
            : base(literalSyntaxToken)
    {
        Type = type;
    }

    public Type Type { get; }
}
