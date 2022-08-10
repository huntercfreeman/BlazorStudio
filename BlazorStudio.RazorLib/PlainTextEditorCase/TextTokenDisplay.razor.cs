using BlazorStudio.ClassLib.Store.CSharpKeywords;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.RazorKeywords;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Microsoft.CodeAnalysis.Text;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class TextTokenDisplay : ComponentBase
{
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<CSharpKeywords> CSharpKeywordsWrap { get; set; } = null!;
    [Inject]
    private IState<RazorKeywords> RazorKeywordsWrap { get; set; } = null!;

    [CascadingParameter] 
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ITextToken TextToken { get; set; } = null!;
    [Parameter, EditorRequired]
    public int StartOfSpanRelativeToRow { get; set; }
    [Parameter, EditorRequired]
    public long StartOfRowSpanRelativeToDocument { get; set; }

    private string TokenClass => GetTokenClass();

    private string GetTokenClass()
    {
        bool isKeyword = false;

        var classBuilder = new StringBuilder();

        var localCSharpKeywords = CSharpKeywordsWrap.Value;

        if (TextToken.Kind == TextTokenKind.Default &&
            localCSharpKeywords.Keywords.Contains(TextToken.PlainText))
        {
            if (!isKeyword)
            {
                // Don't mark as keyword twice redundantly
                isKeyword = true;
                classBuilder.Append("pte_plain-text-editor-text-token-display-keyword");
            }
        }

        var localRazorKeywords = RazorKeywordsWrap.Value;

        foreach (var keywordFunc in localRazorKeywords.KeywordFuncs)
        {
            var classResult = keywordFunc.Invoke(TextToken.PlainText);

            if (!string.IsNullOrWhiteSpace(classResult))
            {
                if (!isKeyword)
                {
                    // Don't mark as keyword twice redundantly
                    isKeyword = true;
                    classBuilder.Append("pte_plain-text-editor-text-token-display-keyword");
                }
            }
        }

        if (!isKeyword)
        {
            var startOfSpanInclusive = StartOfRowSpanRelativeToDocument + StartOfSpanRelativeToRow;
            var endOfSpanExclusive =
                StartOfRowSpanRelativeToDocument + StartOfSpanRelativeToRow + TextToken.PlainText.Length;

            var absoluteFilePathValue = new AbsoluteFilePathStringValue(AbsoluteFilePath);

            if (SolutionStateWrap.Value.FileDocumentMap.TryGetValue(absoluteFilePathValue, out var indexedDocument))
            {
                if (indexedDocument.PropertyDeclarationSyntaxes?.Any() ?? false)
                {
                    foreach (var propertyDeclaration in indexedDocument.PropertyDeclarationSyntaxes)
                    {
                        var typeSpan = propertyDeclaration.Type.Span;
                    
                        if (typeSpan.Intersection(new TextSpan((int) startOfSpanInclusive, TextToken.PlainText.Length))
                            is not null)
                        {
                            classBuilder.Append("pte_plain-text-editor-text-token-display-type");
                        }
                    }
                }
            }
        }

        return classBuilder.ToString();
    }
}
