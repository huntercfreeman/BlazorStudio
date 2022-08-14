using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class GetAllSyntaxCollector : CSharpSyntaxWalker
{
    public List<SyntaxNode> SyntaxNodes { get; } = new();
    public List<SyntaxToken> SyntaxTokens { get; } = new();

    public override void Visit(SyntaxNode? node)
    {
        if (node is not null)
        {
            SyntaxNodes.Add(node);
        }
        
        base.Visit(node);
    }

    public override void VisitToken(SyntaxToken token)
    {
        SyntaxTokens.Add(token);
        
        base.VisitToken(token);
    }
}