using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using TagKind = BlazorStudio.ClassLib.Xml.SyntaxEnums.TagKind;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class TagSyntax : IXmlSyntax
{
    public TagSyntax(
        TagNameSyntax? openTagNameSyntax,
        TagNameSyntax? closeTagNameSyntax,
        ImmutableArray<AttributeSyntax> attributeSyntaxes,
        ImmutableArray<IXmlSyntax> childHtmlSyntaxes,
        TagKind tagKind,
        bool hasSpecialHtmlCharacter = false)
    {
        ChildHtmlSyntaxes = childHtmlSyntaxes;
        HasSpecialHtmlCharacter = hasSpecialHtmlCharacter;
        AttributeSyntaxes = attributeSyntaxes;
        OpenTagNameSyntax = openTagNameSyntax;
        CloseTagNameSyntax = closeTagNameSyntax;
        TagKind = tagKind;
    }

    public TagNameSyntax? OpenTagNameSyntax { get; }
    public TagNameSyntax? CloseTagNameSyntax { get; }
    public ImmutableArray<AttributeSyntax> AttributeSyntaxes { get; }
    public TagKind TagKind { get; }
    public bool HasSpecialHtmlCharacter { get; }
    
    public virtual ImmutableArray<IXmlSyntax> ChildHtmlSyntaxes { get; }
    public virtual HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.Tag;

    public class TagSyntaxBuilder
    {
        public TagNameSyntax? OpenTagNameSyntax { get; set; }
        public TagNameSyntax? CloseTagNameSyntax { get; set; }
        public List<AttributeSyntax> AttributeSyntaxes { get; set; } = new();
        public List<IXmlSyntax> ChildHtmlSyntaxes { get; set; } = new();
        public TagKind TagKind { get; set; }
        public bool HasSpecialHtmlCharacter { get; set; }

        public TagSyntax Build()
        {
            return new TagSyntax(
                OpenTagNameSyntax,
                CloseTagNameSyntax,
                AttributeSyntaxes.ToImmutableArray(),
                ChildHtmlSyntaxes.ToImmutableArray(),
                TagKind,
                HasSpecialHtmlCharacter);
        }
    }
}