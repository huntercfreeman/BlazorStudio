using BlazorStudio.ClassLib.Keyboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorTabDisplay : ComponentBase
{
    [CascadingParameter(Name="ActiveTabIndex")]
    public int ActiveTabIndex { get; set; }

    [Parameter, EditorRequired]
    public int TabIndex { get; set; }
    /// <summary>
    /// Do not use EventCallback it will call StateHasChanged implicitly
    /// causing a second 'StateHasChanged' one from Fluxor one from the EventCallback
    /// </summary>
    [Parameter, EditorRequired]
    public Action<int> SetActiveTabIndexOnClick { get; set; } = null!;
    
    // TODO: Dispose plain text editor
    //
    // [Parameter, EditorRequired]
    // public Action<PlainTextEditorKey> DisposePlainTextEditorOnClick { get; set; } = null!;

    private string IsActiveCssClass => ActiveTabIndex == TabIndex
        ? "bstudio_active"
        : string.Empty;

    private void FireSetActiveTabIndexOnClick()
    {
        SetActiveTabIndexOnClick.Invoke(TabIndex);
    }
    
    private void FireDisposePlainTextEditorOnClick()
    {
        // TODO: Dispose plain text editor
    }
    
    private void EditorTabHandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventArgs.Code ||
            KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE == keyboardEventArgs.Code)
        {
            FireSetActiveTabIndexOnClick();
        }
    }
}