using System.Collections.Immutable;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Experiments.Server.VirtualizeComponents;

public partial class VirtualizeCoordinateSystem<TItem> : ComponentBase, IDisposable
{
    [Inject] 
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [Parameter, EditorRequired] 
    public ICollection<TItem>? Items { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<TItem> ChildContent { get; set; } = null!;

    private VirtualizeItemDimensions? _dimensions;
    private ApplicationException _dimensionsWereNullException = new (
        $"The {nameof(_dimensions)} was null");
    private ElementReference? _topBoundaryElementReference;
    private ElementReference? _bottomBoundaryElementReference;
    private double _topBoundaryHeightInPixels;
    private double _bottomBoundaryHeightInPixels;
    
    private ICollection<TItem>? ResultSet => GetResultSet();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("plainTextEditor.subscribeToVirtualizeScrollEvent",
                _topBoundaryHeightInPixels,
                DotNetObjectReference.Create(this));
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public async Task OnParentElementScrollEvent(ScrollDimensions scrollDimensions)
    {
        var z = 2;
    }
    
    private ICollection<TItem> GetResultSet()
    {
        if (_dimensions is null)
            throw _dimensionsWereNullException;

        // StartIndex
        // Count

        var scrollDimensions = JsRuntime
            .InvokeAsync<ScrollDimensions>("plainTextEditor.getVirtualizeScrollDimensions",
                _topBoundaryHeightInPixels);
        
        // var startIndex = _dimensions.HeightOfScrollableContainer 

        return Items;
    }

    private void OnAfterMeasurementTaken(VirtualizeItemDimensions virtualizeItemDimensions)
    {
        _dimensions = virtualizeItemDimensions;
    }

    public void Dispose()
    {
        // JsRuntime.InvokeVoidAsync("plainTextEditor.disposeVirtualizeScrollEvent",
        //     _topBoundaryHeightInPixels,
        //     DotNetObjectReference.Create(this));
    }
}