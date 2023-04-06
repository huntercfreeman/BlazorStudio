using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;
using TagKind = BlazorStudio.ClassLib.Xml.SyntaxEnums.TagKind;

namespace BlazorStudio.ClassLib.Xml.SyntaxObjects;

public class CommentSyntax : TagSyntax
{
    public CommentSyntax(
        TextEditorTextSpan textEditorTextSpan) 
        : base(
            null,
            null,
            ImmutableArray<AttributeSyntax>.Empty, 
            ImmutableArray<IXmlSyntax>.Empty, 
            TagKind.Text)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    
    public override HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.Comment;
    public override ImmutableArray<IXmlSyntax> ChildHtmlSyntaxes => ImmutableArray<IXmlSyntax>.Empty;
}