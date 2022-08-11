using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class GeneralSyntaxCollector : CSharpSyntaxWalker
{
    public List<PropertyDeclarationSyntax> PropertyDeclaration { get; } = new();
    public List<MethodDeclarationSyntax> MethodDeclaration { get; } = new();
    public List<ArgumentSyntax> ArgumentDeclarations { get; } = new();
    public List<ParameterSyntax> ParameterDeclarations { get; } = new();
    public List<LiteralExpressionSyntax> StringLiteralExpressions { get; } = new();

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {        
        PropertyDeclaration.Add(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodDeclaration.Add(node);
    }

    public override void VisitArgument(ArgumentSyntax node)
    {
        ArgumentDeclarations.Add(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        ParameterDeclarations.Add(node);
    }

    public override void VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.StringLiteralExpression))
        {
            StringLiteralExpressions.Add(node);
        }
    }

    public override void VisitVarPattern(VarPatternSyntax node)
    {
        base.VisitVarPattern(node);
    }
}