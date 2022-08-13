using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class GeneralSyntaxCollector : CSharpSyntaxWalker
{
    public List<PropertyDeclarationSyntax> PropertyDeclarations { get; } = new();
    public List<MethodDeclarationSyntax> MethodDeclarations { get; } = new();
    public List<ArgumentSyntax> ArgumentDeclarations { get; } = new();
    public List<ParameterSyntax> ParameterDeclarations { get; } = new();
    public List<LiteralExpressionSyntax> StringLiteralExpressions { get; } = new();

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {        
        PropertyDeclarations.Add(node);
        
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodDeclarations.Add(node);
        
        base.VisitMethodDeclaration(node);
    }

    public override void VisitArgument(ArgumentSyntax node)
    {
        ArgumentDeclarations.Add(node);
        
        base.VisitArgument(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        ParameterDeclarations.Add(node);
        
        base.VisitParameter(node);
    }

    public override void VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.StringLiteralExpression))
        {
            StringLiteralExpressions.Add(node);
        }
        
        base.VisitLiteralExpression(node);
    }

    public override void VisitVarPattern(VarPatternSyntax node)
    {
        base.VisitVarPattern(node);
    }
}