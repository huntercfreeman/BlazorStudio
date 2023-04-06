using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class TagSyntax : IXmlSyntax
{
    public TagSyntax(
        TagNameSyntax? openTagNameSyntax,
        TagNameSyntax? closeTagNameSyntax,
        ImmutableArray<AttributeSyntax> attributeSyntaxes,
        ImmutableArray<IXmlSyntax> childXmlSyntaxes,
        TagKind tagKind,
        bool hasSpecialXmlCharacter = false)
    {
        ChildXmlSyntaxes = childXmlSyntaxes;
        HasSpecialXmlCharacter = hasSpecialXmlCharacter;
        AttributeSyntaxes = attributeSyntaxes;
        OpenTagNameSyntax = openTagNameSyntax;
        CloseTagNameSyntax = closeTagNameSyntax;
        TagKind = tagKind;
    }

    public TagNameSyntax? OpenTagNameSyntax { get; }
    public TagNameSyntax? CloseTagNameSyntax { get; }
    public ImmutableArray<AttributeSyntax> AttributeSyntaxes { get; }
    public TagKind TagKind { get; }
    public bool HasSpecialXmlCharacter { get; }
    
    public virtual ImmutableArray<IXmlSyntax> ChildXmlSyntaxes { get; }
    public virtual XmlSyntaxKind XmlSyntaxKind => XmlSyntaxKind.Tag;

    public class TagSyntaxBuilder
    {
        public TagNameSyntax? OpenTagNameSyntax { get; set; }
        public TagNameSyntax? CloseTagNameSyntax { get; set; }
        public List<AttributeSyntax> AttributeSyntaxes { get; set; } = new();
        public List<IXmlSyntax> ChildXmlSyntaxes { get; set; } = new();
        public TagKind TagKind { get; set; }
        public bool HasSpecialXmlCharacter { get; set; }

        public TagSyntax Build()
        {
            return new TagSyntax(
                OpenTagNameSyntax,
                CloseTagNameSyntax,
                AttributeSyntaxes.ToImmutableArray(),
                ChildXmlSyntaxes.ToImmutableArray(),
                TagKind,
                HasSpecialXmlCharacter);
        }
    }
}