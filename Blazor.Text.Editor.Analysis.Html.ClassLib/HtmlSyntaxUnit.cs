using System.Collections.Immutable;
using Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;
using Blazor.Text.Editor.Analysis.Shared;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib;

public class HtmlSyntaxUnit
{
    public HtmlSyntaxUnit(
        TagSyntax rootTagSyntax,
        ImmutableArray<TextEditorDiagnostic> textEditorDiagnostics)
    {
        TextEditorDiagnostics = textEditorDiagnostics;
        RootTagSyntax = rootTagSyntax;
    }

    public TagSyntax RootTagSyntax { get; }
    public ImmutableArray<TextEditorDiagnostic> TextEditorDiagnostics { get; }

    public class HtmlSyntaxUnitBuilder
    {
        public TagSyntax RootTagSyntax { get; set; }
        public List<TextEditorDiagnostic> TextEditorDiagnostics { get; } = new();

        public HtmlSyntaxUnit Build()
        {
            return new HtmlSyntaxUnit(
                RootTagSyntax,
                TextEditorDiagnostics.ToImmutableArray());
        }
    }
}