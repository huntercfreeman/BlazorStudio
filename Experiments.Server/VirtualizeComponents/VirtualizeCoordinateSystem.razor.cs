using System.Collections.Immutable;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Experiments.Server.VirtualizeComponents;

public partial class VirtualizeCoordinateSystem<TItem> : ComponentBase
{
    [Parameter, EditorRequired] 
    public ICollection<TItem>? Items { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<TItem> ChildContent { get; set; } = null!;

    private VirtualizeItemDimensions? _dimensionsOfTItem;

    private void OnAfterMeasurementTaken(VirtualizeItemDimensions virtualizeItemDimensions)
    {
        _dimensionsOfTItem = virtualizeItemDimensions;
    }
}