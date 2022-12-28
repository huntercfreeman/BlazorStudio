using BlazorALaCarte.Shared.Keyboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileEditAddress : ComponentBase
{
    [Parameter, EditorRequired]
    public string InitialInputValue { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<string> OnFocusOutCallback { get; set; } = null!;
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

    private void InputTextEditForAddressOnFocusOut()
    {
        if (!_isCancelled)
            OnFocusOutCallback.Invoke(_editForAddressValue);
    }

    private void InputTextEditForAddressOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            _isCancelled = true;
            
            OnEscapeKeyDownCallback.Invoke();
        }
        else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            InputTextEditForAddressOnFocusOut();
        }
    }
}