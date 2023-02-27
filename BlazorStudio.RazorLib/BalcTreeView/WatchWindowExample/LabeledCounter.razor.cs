using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class LabeledCounter : ComponentBase
{
    [Parameter, EditorRequired]
    public string Label { get; set; } = null!;
    
    private int _count;
    
    public void IncrementCount()
    {
        _count++;
        InvokeAsync(StateHasChanged);
    }
}