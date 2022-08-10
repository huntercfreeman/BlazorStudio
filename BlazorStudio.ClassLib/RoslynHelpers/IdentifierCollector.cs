using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class IdentifierCollector : CSharpSyntaxWalker
{
    public List<string> Identifiers { get; } = new();

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        Identifiers.Add(node.Identifier.Text);
    }
}