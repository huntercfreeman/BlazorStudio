using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class GeneralSyntaxCollector : CSharpSyntaxWalker
{
    public GeneralSyntaxCollector()
        : base(SyntaxWalkerDepth.Trivia)
    {
        
    }
    
    public List<PropertyDeclarationSyntax> PropertyDeclarations { get; } = new();
    public List<ClassDeclarationSyntax> ClassDeclarations { get; } = new();
    public List<MethodDeclarationSyntax> MethodDeclarations { get; } = new();
    public List<InvocationExpressionSyntax> InvocationExpressions { get; } = new();
    public List<ArgumentSyntax> ArgumentDeclarations { get; } = new();
    public List<ParameterSyntax> ParameterDeclarations { get; } = new();
    public List<LiteralExpressionSyntax> StringLiteralExpressions { get; } = new();
    public List<SyntaxTrivia> TriviaComments { get; } = new();
    public List<SyntaxToken> Keywords { get; } = new();
    public List<XmlCommentSyntax> XmlComments { get; } = new();

    public override void VisitToken(SyntaxToken token)
    {
        if (token.Kind().ToString().EndsWith("Keyword"))
        {
            Keywords.Add(token);
        }
        
        base.VisitToken(token);
    }

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

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        ClassDeclarations.Add(node);
        
        base.VisitClassDeclaration(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        InvocationExpressions.Add(node);
        
        base.VisitInvocationExpression(node);
    }

    public override void VisitXmlComment(XmlCommentSyntax node)
    {
        XmlComments.Add(node);
        
        base.VisitXmlComment(node);
    }
    
    public override void VisitTrivia(SyntaxTrivia trivia)
    {
        if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
            trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
            trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
            trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
        {
            TriviaComments.Add(trivia);
        }
        
        base.VisitTrivia(trivia);
    }

    public override void VisitVarPattern(VarPatternSyntax node)
    {
        base.VisitVarPattern(node);
    }
}