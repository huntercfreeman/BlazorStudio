using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class PropertyDeclarationCollector : CSharpSyntaxWalker
{
    public List<PropertyDeclarationSyntax> PropertyDeclarations { get; } = new();

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {        
        PropertyDeclarations.Add(node);
    }
}