using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.Virtualize;

public partial class DebugVirtualizeCoordinateSystem<T> : VirtualizeCoordinateSystem<T>
{
    [Parameter]
    public VirtualizeCoordinateSystem<T> VirtualizeCoordinateSystem { get; set; } = null!;
}