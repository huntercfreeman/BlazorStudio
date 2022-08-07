using BlazorStudio.ClassLib.Store.CSharpKeywords;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.RazorKeywords;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Text;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class TextTokenDisplay : ComponentBase
{
    [Inject]
    private IState<CSharpKeywords> CSharpKeywordsWrap { get; set; } = null!;
    [Inject]
    private IState<RazorKeywords> RazorKeywordsWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextToken TextToken { get; set; } = null!;

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

        return classBuilder.ToString();
    }
}
