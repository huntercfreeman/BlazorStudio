using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
    [Parameter, EditorRequired]
    public Action<PlainTextEditorKey> DisposePlainTextEditorOnClick { get; set; } = null!;

    private string IsActiveCssClass => ActiveTabIndex == TabIndex
        ? "bstudio_active"
        : string.Empty;

    private void FireSetActiveTabIndexOnClick()
    {
        SetActiveTabIndexOnClick.Invoke(TabIndex);
    }
    
    private void FireDisposePlainTextEditorOnClick()
    {
        DisposePlainTextEditorOnClick.Invoke(PlainTextEditor.PlainTextEditorKey);
    }
    
    private void EditorTabHandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.NewLineCodes.ENTER_CODE == keyboardEventArgs.Code ||
            KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE == keyboardEventArgs.Code)
        {
            FireSetActiveTabIndexOnClick();
        }
    }
}