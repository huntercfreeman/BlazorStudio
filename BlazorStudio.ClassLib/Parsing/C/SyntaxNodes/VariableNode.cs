using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class VariableNode : ISyntaxNode
{
    public VariableNode(
        Variable variable)
    {
        Variable = variable;
    }
    
    public Variable Variable { get; }

    public ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        Variable,
    }.ToImmutableArray();
    public SyntaxKind SyntaxKind => SyntaxKind.VariableNode;
}