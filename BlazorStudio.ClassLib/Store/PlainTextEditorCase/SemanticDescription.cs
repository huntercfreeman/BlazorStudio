using BlazorStudio.ClassLib.Sequence;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public class SemanticDescription
{
    public SyntaxKind SyntaxKind { get; set; }
    public SequenceKey SequenceKey { get; init; }
    public string CssClassString { get; set; }
}