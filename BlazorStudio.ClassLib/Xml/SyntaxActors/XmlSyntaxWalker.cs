using BlazorTextEditor.RazorLib.Analysis.Html;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;
using AttributeNameSyntax = BlazorStudio.ClassLib.Xml.SyntaxObjects.AttributeNameSyntax;
using AttributeSyntax = BlazorStudio.ClassLib.Xml.SyntaxObjects.AttributeSyntax;
using AttributeValueSyntax = BlazorStudio.ClassLib.Xml.SyntaxObjects.AttributeValueSyntax;
using CommentSyntax = BlazorStudio.ClassLib.Xml.SyntaxObjects.CommentSyntax;
using InjectedLanguageFragmentSyntax = BlazorStudio.ClassLib.Xml.SyntaxObjects.InjectedLanguageFragmentSyntax;
using TagKind = BlazorStudio.ClassLib.Xml.SyntaxEnums.TagKind;
using TagNameSyntax = BlazorStudio.ClassLib.Xml.SyntaxObjects.TagNameSyntax;
using TagSyntax = BlazorStudio.ClassLib.Xml.SyntaxObjects.TagSyntax;

namespace BlazorStudio.ClassLib.Xml.SyntaxActors;

public class XmlSyntaxWalker
{
    public List<SyntaxObjects.AttributeNameSyntax> AttributeNameSyntaxes { get; } = new();
    public List<SyntaxObjects.AttributeValueSyntax> AttributeValueSyntaxes { get; } = new();
    public List<InjectedLanguageFragmentSyntax> InjectedLanguageFragmentSyntaxes { get; } = new();
    public List<TagNameSyntax> TagNameSyntaxes { get; } = new();
    public List<CommentSyntax> CommentSyntaxes { get; } = new();
    public List<TagSyntax> TagSyntaxes { get; } = new();
    
    public void Visit(IXmlSyntax syntaxNode)
    {
        foreach (var child in syntaxNode.ChildHtmlSyntaxes) Visit(child);

        switch (syntaxNode.HtmlSyntaxKind)
        {
            case HtmlSyntaxKind.Tag:
            case HtmlSyntaxKind.InjectedLanguageFragment:
            case HtmlSyntaxKind.TagText:
            {
                var tagSyntax = (TagSyntax)syntaxNode;
                
                if (tagSyntax.OpenTagNameSyntax is not null)
                    VisitTagNameSyntax(tagSyntax.OpenTagNameSyntax);

                if (tagSyntax.CloseTagNameSyntax is not null)
                    VisitTagNameSyntax(tagSyntax.CloseTagNameSyntax);

                if (tagSyntax.TagKind == TagKind.InjectedLanguageCodeBlock)
                {
                    VisitInjectedLanguageFragmentSyntax(
                        (InjectedLanguageFragmentSyntax)tagSyntax);
                }
                
                foreach (var attributeSyntax in tagSyntax.AttributeSyntaxes)
                    Visit(attributeSyntax);
                
                break;
            }
            case HtmlSyntaxKind.Attribute:
            {
                var attributeSyntax = (AttributeSyntax)syntaxNode;

                VisitAttributeNameSyntax(attributeSyntax.AttributeNameSyntax);
                VisitAttributeValueSyntax(attributeSyntax.AttributeValueSyntax);
                
                break;
            }
            case HtmlSyntaxKind.Comment:
            {
                var commentSyntax = (CommentSyntax)syntaxNode;

                VisitCommentSyntax(commentSyntax);
                
                break;
            }
        }
    }

    public void VisitAttributeNameSyntax(AttributeNameSyntax attributeNameSyntax)
    {
        AttributeNameSyntaxes.Add(attributeNameSyntax);
    }

    public void VisitAttributeValueSyntax(AttributeValueSyntax attributeValueSyntax)
    {
        AttributeValueSyntaxes.Add(attributeValueSyntax);
    }

    public void VisitInjectedLanguageFragmentSyntax(InjectedLanguageFragmentSyntax injectedLanguageFragmentSyntax)
    {
        InjectedLanguageFragmentSyntaxes.Add(injectedLanguageFragmentSyntax);
    }

    public void VisitTagNameSyntax(TagNameSyntax tagNameSyntax)
    {
        TagNameSyntaxes.Add(tagNameSyntax);
    }

    public void VisitCommentSyntax(CommentSyntax commentSyntax)
    {
        CommentSyntaxes.Add(commentSyntax);
    }
    
    public void VisitTagSyntax(TagSyntax tagSyntax)
    {
        TagSyntaxes.Add(tagSyntax);
    }
}