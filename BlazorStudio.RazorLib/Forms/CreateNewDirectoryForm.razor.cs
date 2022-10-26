using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Forms;

public partial class CreateNewDirectoryForm : ComponentBase
{
    private string _directoryName = string.Empty;
    private ElementReference _inputElementReference;
    [Parameter]
    public IAbsoluteFilePath ParentDirectory { get; set; } = null!;
    [Parameter]
    public Action<string, string> OnAfterSubmitForm { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public Action OnAfterCancelForm { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) await _inputElementReference.FocusAsync();

        await base.OnAfterRenderAsync(firstRender);
    }

    private void SubmitForm()
    {
        OnAfterSubmitForm(ParentDirectory.GetAbsoluteFilePathString(), _directoryName);
    }

    private void DeclineForm()
    {
        OnAfterCancelForm.Invoke();
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        var keyDownEventRecord = new KeyDownEventRecord(
            keyboardEventArgs.Key,
            keyboardEventArgs.Code,
            keyboardEventArgs.CtrlKey,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.AltKey);

        if (keyDownEventRecord.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            DeclineForm();
        else if (keyDownEventRecord.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE) SubmitForm();
    }
}