using Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class HtmlSyntaxWalker
{
    public List<AttributeNameSyntax> AttributeNameSyntaxes { get; } = new();
    public List<AttributeValueSyntax> AttributeValueSyntaxes { get; } = new();
    public List<CommentSyntax> CommentSyntaxes { get; } = new();
    public List<CustomTagNameSyntax> CustomTagNameSyntaxes { get; } = new();
    public List<EntityReferenceSyntax> EntityReferenceSyntaxes { get; } = new();
    public List<HtmlCodeSyntax> HtmlCodeSyntaxes { get; } = new();
    public List<InjectedLanguageFragmentSyntax> InjectedLanguageFragmentSyntaxes { get; } = new();
    public List<TagNameSyntax> TagNameSyntaxes { get; } = new();
    public List<TagSyntax> TagSyntaxes { get; } = new();

    public void Visit(TagSyntax syntaxNode)
    {
        foreach (var child in syntaxNode.ChildTagSyntaxes)
        {
            Visit(child);
        }
        
        if (syntaxNode.OpenTagNameSyntax is not null)
            VisitTagNameSyntax(syntaxNode.OpenTagNameSyntax);
            
        if (syntaxNode.CloseTagNameSyntax is not null)
            VisitTagNameSyntax(syntaxNode.CloseTagNameSyntax);
        
        if (syntaxNode.TagKind == TagKind.InjectedLanguageCodeBlock)
            VisitInjectedLanguageFragmentSyntax(
                (InjectedLanguageFragmentSyntax)syntaxNode);
    }
    
    public void VisitAttributeNameSyntax(AttributeNameSyntax attributeNameSyntax)
    {
        AttributeNameSyntaxes.Add(attributeNameSyntax);
    }

    public void VisitAttributeValueSyntax(AttributeValueSyntax attributeValueSyntax)
    {
        AttributeValueSyntaxes.Add(attributeValueSyntax);
    }

    public void VisitCommentSyntax(CommentSyntax commentSyntax)
    {
        CommentSyntaxes.Add(commentSyntax);
    }

    public void VisitCustomTagNameSyntax(CustomTagNameSyntax customTagNameSyntax)
    {
        CustomTagNameSyntaxes.Add(customTagNameSyntax);
    }

    public void VisitEntityReferenceSyntax(EntityReferenceSyntax entityReferenceSyntax)
    {
        EntityReferenceSyntaxes.Add(entityReferenceSyntax);
    }

    public void VisitHtmlCodeSyntax(HtmlCodeSyntax htmlCodeSyntax)
    {
        HtmlCodeSyntaxes.Add(htmlCodeSyntax);
    }

    public void VisitInjectedLanguageFragmentSyntax(InjectedLanguageFragmentSyntax injectedLanguageFragmentSyntax)
    {
        InjectedLanguageFragmentSyntaxes.Add(injectedLanguageFragmentSyntax);
    }

    public void VisitTagNameSyntax(TagNameSyntax tagNameSyntax)
    {
        TagNameSyntaxes.Add(tagNameSyntax);
    }

    public void VisitTagSyntax(TagSyntax tagSyntax)
    {
        TagSyntaxes.Add(tagSyntax);
    }
}