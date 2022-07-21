using BlazorStudio.ClassLib.Family;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using BlazorStudio.RazorLib.Virtualize;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class DebuggingVirtualizeCoordinateSystem : ComponentBase
{
    [Parameter]
    public Func<VirtualizeCoordinateSystemResult<DebuggingVirtualizeCoordinateSystemWrapper.DebugRow>>
        GetVirtualizeCoordinateSystemResultFunc { get; set; } = null!;
    [Parameter]
    public Action<VirtualizeCoordinateSystem<DebuggingVirtualizeCoordinateSystemWrapper.DebugRow>>
        OnAfterRenderVirtualizeCoordinateSystemCallback { get; set; } = null!;

    private VirtualizeCoordinateSystem<DebuggingVirtualizeCoordinateSystemWrapper.DebugRow> _virtualizeCoordinateSystem = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            OnAfterRenderVirtualizeCoordinateSystemCallback(_virtualizeCoordinateSystem);
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }
}