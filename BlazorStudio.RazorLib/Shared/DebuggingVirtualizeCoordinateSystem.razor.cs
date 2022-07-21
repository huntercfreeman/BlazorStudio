using BlazorStudio.ClassLib.Family;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class DebuggingVirtualizeCoordinateSystem : ComponentBase
{
    [Parameter]
    public VirtualizeCoordinateSystemResult<DebuggingVirtualizeCoordinateSystemWrapper.DebugRow>
        VirtualizeCoordinateSystemResult { get; set; } = null!;
}