using System.Collections.Immutable;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.SyntaxItems;

public class TagSyntax
{
    public TagSyntax(
        TagNameSyntax? openTagNameSyntax,
        TagNameSyntax? closeTagNameSyntax,
        ImmutableArray<AttributeTupleSyntax> attributeTupleSyntaxes, 
        ImmutableArray<TagSyntax> childTagSyntaxes,
        TagKind tagKind,
        bool hasSpecialHtmlCharacter = false)
    {
        ChildTagSyntaxes = childTagSyntaxes;
        HasSpecialHtmlCharacter = hasSpecialHtmlCharacter;
        AttributeTupleSyntaxes = attributeTupleSyntaxes;
        OpenTagNameSyntax = openTagNameSyntax;
        CloseTagNameSyntax = closeTagNameSyntax;
        TagKind = tagKind;
    }
    
    public TagNameSyntax? OpenTagNameSyntax { get; }
    public TagNameSyntax? CloseTagNameSyntax { get; }
    public ImmutableArray<AttributeTupleSyntax> AttributeTupleSyntaxes { get; }
    public ImmutableArray<TagSyntax> ChildTagSyntaxes { get; }
    public TagKind TagKind { get; }
    public bool HasSpecialHtmlCharacter { get; }

    public class TagSyntaxBuilder
    {
        public TagNameSyntax? OpenTagNameSyntax { get; set; }
        public TagNameSyntax? CloseTagNameSyntax { get; set; }
        public List<AttributeTupleSyntax> AttributeTupleSyntaxes { get; set; } = new();
        public List<TagSyntax> ChildTagSyntaxes { get; set; } = new();
        public TagKind TagKind { get; set; }
        public bool HasSpecialHtmlCharacter { get; set; }

        public TagSyntax Build()
        {
            return new TagSyntax(
                OpenTagNameSyntax,
                CloseTagNameSyntax,
                AttributeTupleSyntaxes.ToImmutableArray(),
                ChildTagSyntaxes.ToImmutableArray(),
                TagKind,
                HasSpecialHtmlCharacter);
        }
    }
}