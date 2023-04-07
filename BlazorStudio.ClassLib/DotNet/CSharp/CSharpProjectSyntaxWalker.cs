using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxObjects;

namespace BlazorStudio.ClassLib.DotNet.CSharp;

public class CSharpProjectSyntaxWalker : XmlSyntaxWalker
{
    public List<TagSyntax> TagSyntaxes { get; } = new();

    public override void VisitTagSyntax(TagSyntax tagSyntax)
    {
        TagSyntaxes.Add(tagSyntax);
        base.VisitTagSyntax(tagSyntax);
    }
}