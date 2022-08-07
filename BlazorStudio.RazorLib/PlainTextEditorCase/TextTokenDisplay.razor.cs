using BlazorStudio.ClassLib.Store.CSharpKeywords;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class TextTokenDisplay : ComponentBase
{
    [Inject]
    private IState<CSharpKeywords> CSharpKeywordsWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextToken TextToken { get; set; } = null!;

    private string TokenClass => GetTokenClass();

    private string GetTokenClass()
    {
        var localCSharpKeywords = CSharpKeywordsWrap.Value;

        if (TextToken.Kind == TextTokenKind.Default &&
            localCSharpKeywords.Keywords.Contains(TextToken.PlainText))
        {
            return "pte_plain-text-editor-text-token-display-keyword";
        }

        return string.Empty;
    }
}
