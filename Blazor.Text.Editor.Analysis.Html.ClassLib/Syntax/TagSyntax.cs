using System.Collections.Immutable;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public class TagSyntax
{
    public TagSyntax(
        TagNameSyntax tagNameSyntax,
        ImmutableArray<AttributeTupleSyntax> attributeTupleSyntaxes, 
        ImmutableArray<TagSyntax> childTagSyntaxes,
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
    public bool HasSpecialHtmlCharacter { get; }

    public class TagSyntaxBuilder
    {
        public TagNameSyntax TagNameSyntax { get; set; }
        public List<AttributeTupleSyntax> AttributeTupleSyntaxes = new();
        public List<TagSyntax> ChildTagSyntaxes = new();
        public bool HasSpecialHtmlCharacter { get; set; }

        public TagSyntax Build()
        {
            return new TagSyntax(
                TagNameSyntax,
                AttributeTupleSyntaxes.ToImmutableArray(),
                ChildTagSyntaxes.ToImmutableArray(),
                HasSpecialHtmlCharacter);
        }
    }
}