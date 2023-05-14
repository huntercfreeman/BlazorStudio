﻿using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Expression;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes;

public class BoundBinaryOperatorNode : ISyntaxNode
{
    public BoundBinaryOperatorNode(
        Type leftOperandType,
        ISyntaxToken operatorToken,
        Type rightOperandType,
        Type resultType)
    {
        LeftOperandType = leftOperandType;
        OperatorToken = operatorToken;
        RightOperandType = rightOperandType;
        ResultType = resultType;
    }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundBinaryOperatorNode;

    public Type LeftOperandType { get; }
    public ISyntaxToken OperatorToken { get; }
    public Type RightOperandType { get; }
    public Type ResultType { get; }
}