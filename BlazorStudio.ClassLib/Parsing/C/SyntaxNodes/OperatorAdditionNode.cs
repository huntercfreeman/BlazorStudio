using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class OperatorAdditionNode : OperatorNode, ISyntaxNode
{
    public OperatorAdditionNode(
        PlusToken plusToken)
    {
        PlusToken = plusToken;
    }

    public PlusToken PlusToken { get; }
    public override SyntaxKind SyntaxKind => SyntaxKind.OperatorAdditionNode;
    public override ImmutableArray<ISyntax> Children => new ISyntax[]
    {
        PlusToken
    }.ToImmutableArray();
}