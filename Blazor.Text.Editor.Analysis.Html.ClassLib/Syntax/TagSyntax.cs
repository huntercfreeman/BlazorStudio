using System.Collections.Immutable;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public class TagSyntax
{
    public TagSyntax(
        TagNameSyntax tagNameSyntax,
        ImmutableArray<AttributeTupleSyntax> attributeTupleSyntaxes, 
        ImmutableArray<TagSyntax> childTagSyntaxes,
        TagKind tagKind,
        bool hasSpecialHtmlCharacter = false)
    {
        ChildTagSyntaxes = childTagSyntaxes;
        HasSpecialHtmlCharacter = hasSpecialHtmlCharacter;
        AttributeTupleSyntaxes = attributeTupleSyntaxes;
        TagNameSyntax = tagNameSyntax;
    }
    
    public TagNameSyntax TagNameSyntax { get; }
    public ImmutableArray<AttributeTupleSyntax> AttributeTupleSyntaxes { get; }
    public ImmutableArray<TagSyntax> ChildTagSyntaxes { get; }
    public TagKind TagKind { get; }
    public bool HasSpecialHtmlCharacter { get; }

    public class TagSyntaxBuilder
    {
        public TagNameSyntax TagNameSyntax { get; set; }
        public List<AttributeTupleSyntax> AttributeTupleSyntaxes { get; set; } = new();
        public List<TagSyntax> ChildTagSyntaxes { get; set; } = new();
        public TagKind TagKind { get; set; }
        public bool HasSpecialHtmlCharacter { get; set; }

        public TagSyntax Build()
        {
            return new TagSyntax(
                TagNameSyntax,
                AttributeTupleSyntaxes.ToImmutableArray(),
                ChildTagSyntaxes.ToImmutableArray(),
                TagKind,
                HasSpecialHtmlCharacter);
        }
    }
}