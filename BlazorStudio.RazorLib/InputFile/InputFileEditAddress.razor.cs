using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileEditAddress : ComponentBase
{
    [Parameter, EditorRequired]
    public string InitialInputValue { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<string> OnFocusOutCallback { get; set; } = null!;

    private string _editForAddressValue = string.Empty;
    private ElementReference? _inputTextEditForAddressElementReference;

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
        OnFocusOutCallback.Invoke(_editForAddressValue);
    }
}