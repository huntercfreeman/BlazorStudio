using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorTabDisplay : ComponentBase
{
    [CascadingParameter(Name="ActiveTabIndex")]
    public int ActiveTabIndex { get; set; }

    [Parameter, EditorRequired]
    public int TabIndex { get; set; }
    [Parameter, EditorRequired]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;
    /// <summary>
    /// Do not use EventCallback it will call StateHasChanged implicitly
    /// causing a second 'StateHasChanged' one from Fluxor one from the EventCallback
    /// </summary>
    [Parameter, EditorRequired]
    public Action<int> SetActiveTabIndexOnClick { get; set; } = null!;

    private string IsActiveCssClass => ActiveTabIndex == TabIndex
        ? "bstudio_active"
        : string.Empty;

    private void FireSetActiveTabIndexOnClick()
    {
        SetActiveTabIndexOnClick.Invoke(TabIndex);
    }
}