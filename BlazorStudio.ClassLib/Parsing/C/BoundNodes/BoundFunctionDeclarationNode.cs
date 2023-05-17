using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.BoundNodes;

public class BoundFunctionDeclarationNode : ISyntaxNode
{
    public BoundFunctionDeclarationNode(
        BoundTypeNode boundTypeNode,
        ISyntaxToken identifierToken)
    {
        BoundTypeNode = boundTypeNode;
        IdentifierToken = identifierToken;

        Children = new ISyntax[]
        {
            BoundTypeNode,
            IdentifierToken
        }.ToImmutableArray();
    }
    
    public BoundFunctionDeclarationNode(
        BoundTypeNode boundTypeNode,
        ISyntaxToken identifierToken,
        CompilationUnit functionBodyCompilationUnit)
    {
        BoundTypeNode = boundTypeNode;
        IdentifierToken = identifierToken;
        FunctionBodyCompilationUnit = functionBodyCompilationUnit;

        Children = new ISyntax[]
        {
            BoundTypeNode,
            IdentifierToken,
            FunctionBodyCompilationUnit
        }.ToImmutableArray();
    }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionDeclarationNode;

    public BoundTypeNode BoundTypeNode { get; }
    public ISyntaxToken IdentifierToken { get; }
    public CompilationUnit? FunctionBodyCompilationUnit { get; }

    public BoundFunctionDeclarationNode WithFunctionBody(
        CompilationUnit functionBodyCompilationUnit)
    {
        return new BoundFunctionDeclarationNode(
            BoundTypeNode,
            IdentifierToken,
            functionBodyCompilationUnit);
    }
}
