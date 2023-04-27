using System.Collections.Immutable;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

public class TypedLiteralExpressionNode : TypedExpressionNode
{
    public TypedLiteralExpressionNode(
        ISyntaxToken syntaxToken)
    {
        SyntaxToken = syntaxToken;
    }

    public ISyntaxToken SyntaxToken { get; }
    public object Value { get; }
    
    public override Type ResultType { get; }
    public override SyntaxKind SyntaxKind => SyntaxKind.TypedLiteralExpressionNode; 
    public override ImmutableArray<ISyntax> Children => ImmutableArray<ISyntax>.Empty;
}