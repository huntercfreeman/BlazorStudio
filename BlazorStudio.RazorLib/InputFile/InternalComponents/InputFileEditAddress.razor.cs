using BlazorCommon.RazorLib.Keyboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile.InternalComponents;

public partial class InputFileEditAddress : ComponentBase
{
    [Parameter, EditorRequired]
    public string InitialInputValue { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<string, Task> OnFocusOutCallbackAsync { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action OnEscapeKeyDownCallback { get; set; } = null!;

    private string _editForAddressValue = string.Empty;
    private ElementReference? _inputTextEditForAddressElementReference;
    private bool _isCancelled;

    protected override void OnInitialized()
    {
        _editForAddressValue = InitialInputValue;
        
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _inputTextEditForAddressElementReference?.FocusAsync();
        }
        
        base.OnAfterRender(firstRender);
    }

    private async Task InputTextEditForAddressOnFocusOutAsync()
    {
        if (!_isCancelled)
            await OnFocusOutCallbackAsync.Invoke(_editForAddressValue);
    }

    private async Task InputTextEditForAddressOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            _isCancelled = true;
            
            OnEscapeKeyDownCallback.Invoke();
        }
        else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            await InputTextEditForAddressOnFocusOutAsync();
        }
    }
}