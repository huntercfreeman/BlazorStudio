using System.Collections.Concurrent;
using BlazorStudio.ClassLib.Sequence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public partial class VirtualizeCoordinateSystemExperimental<TItem> : ComponentBase, IDisposable
{
    [Inject] 
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    /// <summary>
    /// The <see cref="ICollection{T}"/> of <see cref="TItem"/>.
    /// The virtualization result pulls a range of <see cref="TItem"/> from
    /// this collection as specified by the scroll position and etc...
    /// then renders those <see cref="TItem"/>
    /// </summary>
    [Parameter, EditorRequired] 
    public ICollection<TItem>? Items { get; set; } = null!;
    /// <summary>
    /// The <see cref="RenderFragment"/> to render foreach <see cref="TItem"/>
    /// in the virtualization result.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment<TItem> ChildContent { get; set; } = null!;

    /// <summary>
    /// Same purpose as <see cref="Virtualize{TItem}"/>
    /// <br/>--<br/>
    /// Gets or sets a value that determines how many additional items will be rendered before
    /// and after the visible region. This help to reduce the frequency of rendering during
    /// scrolling. However, higher values mean that more elements will be present in the page.
    /// <br/>--<br/>
    /// Default value: <see cref="GetDefaultScrollThrottleDelayTimeSpan"/>
    /// </summary>
    [Parameter, EditorRequired]
    public int OverscanCount { get; set; } = GetDefaultOverScanCount();
    /// <summary>
    /// If the <see cref="VirtualizeItemDimensions"/> are changed and one wishes to remeasure
    /// the scrollable parent html element. 
    /// <br/>--<br/>
    /// Then pass in a <see cref="SequenceKey.NewSequenceKey"/> and the parent html element
    /// will be remeasured.
    /// </summary>
    [Parameter, EditorRequired]
    public SequenceKey DimensionsSequenceKey { get; set; } = null!;
    /// <summary>
    /// If the <see cref="Items"/> are changed and one wishes to recalculate the current items that
    /// should be displayed given the scroll position.
    /// <br/>--<br/>
    /// Then pass in a <see cref="SequenceKey.NewSequenceKey"/> and the items will be recalculated.
    /// </summary>
    [Parameter, EditorRequired]
    public SequenceKey ItemsSequenceKey { get; set; } = null!;
    /// <summary>
    /// To avoid an overly abundant amount of scroll events there is a default throttling delay: <see cref="GetDefaultScrollThrottleDelayTimeSpan"/>
    /// <br/>--<br/>
    /// Use this parameter to set a different throttling delay if desired.
    /// </summary>
    [Parameter]
    public TimeSpan ScrollThrottleDelayTimeSpan { get; set; } = GetDefaultScrollThrottleDelayTimeSpan();
    
    private static TimeSpan GetDefaultScrollThrottleDelayTimeSpan() => TimeSpan.FromMilliseconds(100);
    private static int GetDefaultOverScanCount() => 3;

    private Guid _virtualizeCoordinateSystemIdentifier = Guid.NewGuid();
    private bool _forceGetDimensions = false;
    private VirtualizeItemDimensions? _dimensions;
    private ApplicationException _dimensionsWereNullException = new ($"The {nameof(_dimensions)} was null");
    private ApplicationException _itemsWereNullException = new ($"The {nameof(Items)} was null");
    
    /// <summary>
    /// In addition to the typical functionality of being a <see cref="VirtualizeBoundary"/>
    /// this is used to find the <see cref="VirtualizeCoordinateSystemExperimental{TItem}"/>
    /// within the HTML as the scrollable container is provided by the user of this component
    /// the component thereby cannnot find the scrollable container except by getting the parent element of
    /// a <see cref="VirtualizeBoundary"/>
    /// </summary>
    private VirtualizeBoundaryDisplay? _topBoundaryVirtualizeBoundaryDisplay;
    
    private ScrollDimensions? _scrollDimensions;
    private ConcurrentStack<ScrollDimensions> _scrollEventConcurrentStack = new();
    private SemaphoreSlim _handleScrollEventSemaphoreSlim = new(1, 1);
    private Task _throttleDelayTask = Task.CompletedTask;
    private SequenceKey? _previousItemsSequenceKey;
    private SequenceKey? _previousDimensionsSequenceKey;

    private VirtualizeRenderData<TItem> _virtualizeRenderData = new();

    protected override async Task OnParametersSetAsync()
    {
        if (_previousDimensionsSequenceKey is not null && _previousDimensionsSequenceKey != DimensionsSequenceKey)
        {
            // "_previousDimensionsSequenceKey is not null" is done to avoid the first OnParametersSetAsync

            if (Items is null)
                throw _itemsWereNullException;

            _forceGetDimensions = true;
        }

        _previousItemsSequenceKey = DimensionsSequenceKey;
        
        if (_previousItemsSequenceKey is not null && _previousItemsSequenceKey != ItemsSequenceKey)
        {
            // "_previousSequenceKey is not null" is done to avoid the first OnParametersSetAsync

            if (Items is null)
                throw _itemsWereNullException;
        
            if (_scrollDimensions is not null)
                await GetResultSetAsync();
        }

        _previousItemsSequenceKey = ItemsSequenceKey;
        
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SubscribeToVirtualizeScrollEvent();

            // The JavaScript cannot be invoked during the first
            // measurement as on after first render did not occur yet which is normal Blazor functionality.
            //
            // Therefore the measurement must be taken a second time after the first render to get
            // the JavaScript calls (which were initially skipped by returning early) to get invoked.
            await OnAfterMeasurementTaken(_dimensions);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// This method is called during <see cref="OnAfterRenderAsync"/>
    /// when it is the firstRender.
    /// <br/>--<br/>
    /// This method is public as I feel there might be some edge cases where
    /// someone will need to re-subscribe.
    /// <br/>--<br/>
    /// Calling this method when one is not in the edge case of needing to re-subscribe
    /// will result in multiple subscriptions.
    /// <br/>--<br/>
    /// I am not sure of an edge case that would require re-subscribing as of writing this comment
    /// but it is a lingering thought in the back of my mind that there could be one.
    /// Therefore this method is here and is public.
    /// </summary>
    public async Task SubscribeToVirtualizeScrollEvent()
    {
        if (_topBoundaryVirtualizeBoundaryDisplay is null)
            return;
        
        await JsRuntime.InvokeVoidAsync("plainTextEditor.subscribeToVirtualizeScrollEvent",
            _topBoundaryVirtualizeBoundaryDisplay.BoundaryElementReference,
            DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public async Task OnParentElementScrollEvent(ScrollDimensions scrollDimensions)
    {
        // TODO: ensure this semaphore logic does not lose the most recent event in any cases.
        _scrollEventConcurrentStack.Push(scrollDimensions);

        ScrollDimensions? mostRecentScrollDimensions = null;
        
        try
        {
            await _handleScrollEventSemaphoreSlim.WaitAsync();

            await _throttleDelayTask;
            
            if (!_scrollEventConcurrentStack.TryPop(out mostRecentScrollDimensions))
                return;

            _scrollEventConcurrentStack.Clear();

            _throttleDelayTask = Task.Delay(ScrollThrottleDelayTimeSpan);
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
            return;
        
        var startIndex = _scrollDimensions.ScrollTop / _dimensions.HeightOfItemInPixels;
        var count = _dimensions.HeightOfScrollableContainerInPixels / _dimensions.HeightOfItemInPixels;
        
        var totalHeight = _dimensions.HeightOfItemInPixels * Items.Count;
        
        var topBoundaryHeight = _scrollDimensions.ScrollTop;
        
        var bottomBoundaryHeight = totalHeight - topBoundaryHeight - _dimensions.HeightOfScrollableContainerInPixels;

        var heightOfRenderedContent = _dimensions.HeightOfScrollableContainerInPixels;
        
        // Apply OverscanCount
        if (OverscanCount > 0)
        {
            // Apply Top Overscan
            {
                var overallFirstIndex = 0;

                var resultsMapFirstIndex = startIndex;

                var topAvailableOverscan = resultsMapFirstIndex - overallFirstIndex;

                if (topAvailableOverscan > 0)
                {
                    var overscan = Math.Min(topAvailableOverscan, OverscanCount);

                    startIndex -= overscan;
                    count += overscan;
                    
                    var extraRenderedHeight = overscan * _dimensions.HeightOfItemInPixels;

                    heightOfRenderedContent += extraRenderedHeight;
                    topBoundaryHeight -= extraRenderedHeight;
                }
            }

            // Apply Bottom Overscan
            {
                var overallLastIndex = Items.Count - 1;
                
                var resultsMapLastIndex = startIndex + count - 1;
            
                var bottomAvailableOverscan = overallLastIndex - resultsMapLastIndex;

                if (bottomAvailableOverscan > 0)
                {
                    var overscan = Math.Min(bottomAvailableOverscan, OverscanCount);

                    count += overscan;

                    var extraRenderedHeight = overscan * _dimensions.HeightOfItemInPixels;
                
                    heightOfRenderedContent += extraRenderedHeight;
                    bottomBoundaryHeight -= extraRenderedHeight;
                }
            }
        }

        // Round startIndex down
        {
            var percentageOfAnItemLost = startIndex - Math.Truncate(startIndex);

            var percentageOfAnItemGained = .1 - percentageOfAnItemLost;

            var heightShift = _dimensions.HeightOfItemInPixels * percentageOfAnItemGained;

            topBoundaryHeight -= heightShift;
            heightOfRenderedContent += heightShift;
        }
        
        // Round count up
        {
            var percentageOfAnItemLost = count - Math.Truncate(count);

            var percentageOfAnItemGained = .1 - percentageOfAnItemLost;

            var heightShift = _dimensions.HeightOfItemInPixels * percentageOfAnItemGained;

            heightOfRenderedContent += heightShift;
            bottomBoundaryHeight -= heightShift;
        }

        // Experiencing a negative top style for topVirtualizeBoundary when scrolling back to top
        {
            topBoundaryHeight = Math.Max(topBoundaryHeight, 0);
        }
        
        var results = Items
            .Skip((int) Math.Floor(startIndex))
            .Take((int) Math.Ceiling(count))
            .Select((item, i) => 
                new VirtualizeItemWrapper<TItem>(item, 
                    topBoundaryHeight + (i * _dimensions.HeightOfItemInPixels), 
                    100))
            .ToArray();
        
        var bottomVirtualizeBoundary = _virtualizeRenderData.BottomVirtualizeBoundary with
        {
            HeightInPixels = bottomBoundaryHeight,
            OffsetFromTopInPixels = topBoundaryHeight + heightOfRenderedContent
        };
        
        var topVirtualizeBoundary = _virtualizeRenderData.TopVirtualizeBoundary with
        {
            HeightInPixels = topBoundaryHeight,
            OffsetFromTopInPixels = 0
        };

        _virtualizeRenderData = _virtualizeRenderData with
        {
            BottomVirtualizeBoundary = bottomVirtualizeBoundary,
            TopVirtualizeBoundary = topVirtualizeBoundary,
            VirtualizeItemWrappers = results
        }; 
        
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnAfterMeasurementTaken(VirtualizeItemDimensions? virtualizeItemDimensions)
    {
        // Before OnAfterRenderAsync JavaScript cannot be ran
        // Conveniently before OnAfterRenderAsync virtualizeItemDimensions is passed in as null
        if (virtualizeItemDimensions is null)
            return;
        
        _forceGetDimensions = false;
        
        _dimensions = virtualizeItemDimensions;

        if (_topBoundaryVirtualizeBoundaryDisplay is null)
            return;
        
        _scrollDimensions = await JsRuntime.InvokeAsync<ScrollDimensions>("plainTextEditor.getVirtualizeScrollDimensions",
            _topBoundaryVirtualizeBoundaryDisplay.BoundaryElementReference);

        await OnParentElementScrollEvent(_scrollDimensions);
    }

    public void Dispose()
    {
    }
}