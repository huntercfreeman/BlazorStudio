using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class GeneralSyntaxCollector : CSharpSyntaxWalker
{
    public List<PropertyDeclarationSyntax> PropertyDeclarations { get; } = new();
    public List<MethodDeclarationSyntax> MethodDeclarations { get; } = new();

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {        
        PropertyDeclarations.Add(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodDeclarations.Add(node);
    }
}