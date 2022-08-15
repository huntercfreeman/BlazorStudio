using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public partial class VirtualizeCoordinateSystemExperimental<TItem> : ComponentBase, IDisposable
{
    [Inject] 
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [Parameter, EditorRequired] 
    public ICollection<TItem>? Items { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<TItem> ChildContent { get; set; } = null!;

    private Guid _virtualizeCoordinateSystemIdentifier = Guid.NewGuid();
    private VirtualizeItemDimensions? _dimensions;
    private ApplicationException _dimensionsWereNullException = new (
        $"The {nameof(_dimensions)} was null");
    private ElementReference? _topBoundaryElementReference;
    private ElementReference? _bottomBoundaryElementReference;
    private double _topBoundaryHeightInPixels;
    private double _bottomBoundaryHeightInPixels;
    private ScrollDimensions? _scrollDimensions;
    private ConcurrentStack<ScrollDimensions> _scrollEventConcurrentStack = new();
    private SemaphoreSlim _handleScrollEventSemaphoreSlim = new(1, 1);
    private int _scrollCounter;
    private int _throttledScrollCounter;
    private TimeSpan _throttleDelayTimeSpan = TimeSpan.FromMilliseconds(100);
    private Task _throttleDelayTask = Task.CompletedTask;

    // TODO: Make a class for this. I need to ensure the top and bottom boundaries rerender with the same batch of data
    private (double topBoundaryHeightInPixels, double bottomBoundaryHeightInPixels, ICollection<TItem>? resultSet) _renderData;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("plainTextEditor.subscribeToVirtualizeScrollEvent",
                _topBoundaryElementReference,
                DotNetObjectReference.Create(this));
            
            var firstScrollDimensions = await JsRuntime.InvokeAsync<ScrollDimensions>("plainTextEditor.getVirtualizeScrollDimensions",
                _topBoundaryElementReference);

            await OnParentElementScrollEvent(firstScrollDimensions);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public async Task OnParentElementScrollEvent(ScrollDimensions scrollDimensions)
    {
        _scrollEventConcurrentStack.Push(scrollDimensions);
        
        _scrollCounter++;
        await InvokeAsync(StateHasChanged);

        ScrollDimensions? mostRecentScrollDimensions = null;
        
        try
        {
            await _handleScrollEventSemaphoreSlim.WaitAsync();

            await _throttleDelayTask;
            
            if (!_scrollEventConcurrentStack.TryPop(out mostRecentScrollDimensions))
            {
                return;
            }

            _scrollEventConcurrentStack.Clear();

            _throttledScrollCounter++;

            _throttleDelayTask = Task.Delay(_throttleDelayTimeSpan);
        }
        finally
        {
            _handleScrollEventSemaphoreSlim.Release();
        }

        _scrollDimensions = mostRecentScrollDimensions;

        await GetResultSetAsync();
    }
    
    private async Task GetResultSetAsync()
    {
        if (_dimensions is null)
            throw _dimensionsWereNullException;

        if (_scrollDimensions is null)
        {
            return;
        }
        
        var startIndex = _scrollDimensions.ScrollTop / _dimensions.HeightOfTItemInPixels;
        var count = _dimensions.HeightOfScrollableContainer / _dimensions.HeightOfTItemInPixels;
        
        var totalHeight = _dimensions.HeightOfTItemInPixels * Items.Count;
        
        var topBoundaryHeight = _scrollDimensions.ScrollTop;

        topBoundaryHeight = Math.Min(topBoundaryHeight,
            totalHeight - _dimensions.HeightOfScrollableContainer);
        
        var bottomBoundaryHeight = totalHeight - topBoundaryHeight - _dimensions.HeightOfScrollableContainer;

        if (bottomBoundaryHeight < _dimensions.HeightOfTItemInPixels)
        {
            bottomBoundaryHeight = 0;
        }
        
        var results = Items
            .Skip((int) Math.Ceiling(startIndex))
            .Take((int) Math.Ceiling(count))
            .ToArray();
        
        _renderData = (topBoundaryHeight, bottomBoundaryHeight, results); 
        
        await InvokeAsync(StateHasChanged);
    }

    private void OnAfterMeasurementTaken(VirtualizeItemDimensions virtualizeItemDimensions)
    {
        _dimensions = virtualizeItemDimensions;
    }

    public void Dispose()
    {
    }
}