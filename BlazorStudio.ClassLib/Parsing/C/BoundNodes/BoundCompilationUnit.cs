using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class BoundCompilationUnit
{
    public BoundCompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children)
    {
        IsExpression = isExpression;
        Children = children;
    }

    public bool IsExpression { get; }
    public ImmutableArray<ISyntax> Children { get; }
}
