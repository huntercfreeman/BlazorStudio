using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BlazorStudio.ClassLib.SyntaxHighlighting;

public class GeneralSyntaxCollector : CSharpSyntaxWalker
{
    public GeneralSyntaxCollector()
        : base(SyntaxWalkerDepth.Trivia)
    {
    }
    
    public List<PropertyDeclarationSyntax> PropertyDeclarationSyntaxes { get; } = new();
    public List<ClassDeclarationSyntax> ClassDeclarationSyntaxes { get; } = new();
    public List<MethodDeclarationSyntax> MethodDeclarationSyntaxes { get; } = new();
    public List<InvocationExpressionSyntax> InvocationExpressionSyntaxes { get; } = new();
    public List<ArgumentSyntax> ArgumentSyntaxes { get; } = new();
    public List<ParameterSyntax> ParameterSyntaxes { get; } = new();
    public List<LiteralExpressionSyntax> StringLiteralExpressionSyntaxes { get; } = new();
    public List<SyntaxTrivia> SyntaxTrivias { get; } = new();
    public List<SyntaxToken> KeywordSyntaxTokens { get; } = new();
    public List<TextSpan> VarTextSpans { get; } = new();
    public List<XmlCommentSyntax> XmlCommentSyntaxes { get; } = new();

    public override void VisitToken(SyntaxToken token)
    {
        if (token.Kind().ToString().EndsWith("Keyword"))
        {
            KeywordSyntaxTokens.Add(token);
        }
        
        base.VisitToken(token);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {        
        PropertyDeclarationSyntaxes.Add(node);
        
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodDeclarationSyntaxes.Add(node);
        
        base.VisitMethodDeclaration(node);
    }

    public override void VisitArgument(ArgumentSyntax node)
    {
        ArgumentSyntaxes.Add(node);
        
        base.VisitArgument(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        ParameterSyntaxes.Add(node);
        
        base.VisitParameter(node);
    }

    public override void VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.StringLiteralExpression))
        {
            StringLiteralExpressionSyntaxes.Add(node);
        }
        
        base.VisitLiteralExpression(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        ClassDeclarationSyntaxes.Add(node);
        
        base.VisitClassDeclaration(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        InvocationExpressionSyntaxes.Add(node);
        
        base.VisitInvocationExpression(node);
    }

    public override void VisitXmlComment(XmlCommentSyntax node)
    {
        XmlCommentSyntaxes.Add(node);
        
        base.VisitXmlComment(node);
    }
    
    public override void VisitTrivia(SyntaxTrivia trivia)
    {
        if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
            trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
            trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
            trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
        {
            SyntaxTrivias.Add(trivia);
        }
        
        base.VisitTrivia(trivia);
    }

    public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        if (node.Declaration.Type.IsVar)
        {
            VarTextSpans.Add(node.Declaration.Type.Span);
        }
        
        base.VisitLocalDeclarationStatement(node);
    }
}