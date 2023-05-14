using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Binder
{
    private ISyntaxNode? _currentNode;
    private BoundCompilationUnitBuilder _boundCompilationUnitBuilder = new();

    public BoundCompilationUnit BoundCompilationUnit => _boundCompilationUnitBuilder.Build();

    public void BindLiteralExpressionNode(LiteralExpressionNode literalExpressionNode)
    {
        var type = literalExpressionNode.LiteralSyntaxToken.SyntaxKind switch
        {
            SyntaxKind.NumericLiteralToken => typeof(Int32),
            SyntaxKind.StringLiteralToken => typeof(string),
            _ => throw new NotImplementedException(),
        };

        var boundLiteralExpressionNode = new BoundLiteralExpressionNode(
            literalExpressionNode.LiteralSyntaxToken,
            type);

        if (_currentNode is null)
        {
            _currentNode = boundLiteralExpressionNode;
            _boundCompilationUnitBuilder.Children.Add(boundLiteralExpressionNode);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
