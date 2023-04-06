using System.Collections.Immutable;
using BlazorStudio.ClassLib.Xml.SyntaxEnums;
using BlazorTextEditor.RazorLib.Lexing;

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
    
    public override XmlSyntaxKind XmlSyntaxKind => XmlSyntaxKind.Comment;
    public override ImmutableArray<IXmlSyntax> ChildXmlSyntaxes => ImmutableArray<IXmlSyntax>.Empty;
}