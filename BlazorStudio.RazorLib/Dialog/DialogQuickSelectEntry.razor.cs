using BlazorStudio.ClassLib.Store.DialogCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogQuickSelectEntry : ComponentBase
{
    [CascadingParameter(Name="ActiveEntryIndex")]
    public int ActiveEntryIndex { get; set; }
    
    [Parameter]
    public int EntryIndex { get; set; }
    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private ElementReference? _dialogQuickSelectEntry;
    
    protected override async Task OnParametersSetAsync()
    {
        if (_dialogQuickSelectEntry is not null &&
            EntryIndex == ActiveEntryIndex)
        {
            await _dialogQuickSelectEntry.Value.FocusAsync();
        }
        
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_dialogQuickSelectEntry is not null &&
                EntryIndex == ActiveEntryIndex)
            {
                await _dialogQuickSelectEntry.Value.FocusAsync();
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
}