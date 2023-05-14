using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class CompilationUnit
{
    public CompilationUnit(
        bool isExpression,
        ImmutableArray<ISyntax> children,
        BoundCompilationUnit boundCompilationUnit)
    {
        IsExpression = isExpression;
        Children = children;
        BoundCompilationUnit = boundCompilationUnit;
    }

    public bool IsExpression { get; }
    public ImmutableArray<ISyntax> Children { get; }
    public BoundCompilationUnit BoundCompilationUnit { get; }
}
