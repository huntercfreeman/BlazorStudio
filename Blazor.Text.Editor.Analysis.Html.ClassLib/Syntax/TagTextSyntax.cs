using System.Collections.Immutable;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public class TagTextSyntax : TagSyntax
{
    public TagTextSyntax(
        TagNameSyntax tagNameSyntax,
        ImmutableArray<AttributeTupleSyntax> attributeTupleSyntaxes,
        ImmutableArray<TagSyntax> childTagSyntaxes,
        string value,
        bool hasSpecialHtmlCharacter = false) 
        : base(
            tagNameSyntax, 
            attributeTupleSyntaxes, 
            childTagSyntaxes, 
            TagKind.Text,
            hasSpecialHtmlCharacter)
    {
        Value = value;
    }

    public string Value { get; }
}