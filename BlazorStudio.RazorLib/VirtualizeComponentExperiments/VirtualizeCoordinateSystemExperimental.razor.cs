﻿using System.Collections.Concurrent;
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
    
    private static int GetDefaultOverScanCount() => 3;

    private Guid _virtualizeCoordinateSystemIdentifier = Guid.NewGuid();
    private bool _forceGetDimensions = false;
    private VirtualizeItemDimensions? _dimensions;
    private ApplicationException _dimensionsWereNullException = new ($"The {nameof(_dimensions)} was null");
    private ApplicationException _itemsWereNullException = new ($"The {nameof(Items)} was null");
    private int _onIntersectionThresholdChangedCounter;
    private int _pushUserInterfaceModificationAsyncCounter;
    private readonly ConcurrentStack<Func<Task>> _userInterfaceModificationConcurrentStack = new();
    private readonly SemaphoreSlim _executeUserInterfaceModificationSemaphoreSlim = new(1, 1);
    private readonly SemaphoreSlim _clearOutdatedUserInterfaceModificationSemaphoreSlim = new(1, 1);

    /// <summary>
    /// In addition to the typical functionality of being a <see cref="VirtualizeBoundary"/>
    /// this is used to find the <see cref="VirtualizeCoordinateSystemExperimental{TItem}"/>
    /// within the HTML as the scrollable container is provided by the user of this component
    /// the component thereby cannot find the scrollable container except by getting the parent element of
    /// a <see cref="VirtualizeBoundary"/>
    /// </summary>
    private VirtualizeBoundaryDisplay? _topBoundaryBlazorComponent;
    private VirtualizeBoundaryDisplay? _bottomBoundaryBlazorComponent;

    private List<VirtualizeBoundaryDisplay?> GetVirtualizeBoundaryBlazorComponents() => new()
    {
        _topBoundaryBlazorComponent,
        _bottomBoundaryBlazorComponent
    };

    private ScrollDimensions? _scrollDimensions;
    private SequenceKey? _previousItemsSequenceKey;
    private SequenceKey? _previousDimensionsSequenceKey;

    private VirtualizeItemWrapper<TItem>[] _virtualizeItemWrappers = 
        Array.Empty<VirtualizeItemWrapper<TItem>>();

    private VirtualizeBoundary _topVirtualizeBoundary = new(VirtualizeBoundaryKind.Top);
    private VirtualizeBoundary _bottomVirtualizeBoundary = new(VirtualizeBoundaryKind.Bottom);

    protected override async Task OnParametersSetAsync()
    {
        if (_previousDimensionsSequenceKey is not null && _previousDimensionsSequenceKey != DimensionsSequenceKey)
        {
            // "_previousDimensionsSequenceKey is not null" is done to avoid the first OnParametersSetAsync

            if (Items is null)
                throw _itemsWereNullException;

            _forceGetDimensions = true;
        }

        _previousDimensionsSequenceKey = DimensionsSequenceKey;
        
        if (_previousItemsSequenceKey is not null && _previousItemsSequenceKey != ItemsSequenceKey)
        {
            // "_previousSequenceKey is not null" is done to avoid the first OnParametersSetAsync

            if (Items is null)
                throw _itemsWereNullException;
        
            if (_scrollDimensions is not null)
                await OnIntersectionObserverThresholdChanged(_scrollDimensions);
        }

        _previousItemsSequenceKey = ItemsSequenceKey;
        
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializeVirtualizeIntersectionObserver();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
    
    private async Task PushUserInterfaceModificationAsync(Func<Task> func)
    {
        try
        {
            // Only allow Push when the Stack is not being Cleared
            // as to not lose the most recent modification due to timing
            await _clearOutdatedUserInterfaceModificationSemaphoreSlim.WaitAsync();
            
            _userInterfaceModificationConcurrentStack.Push(func);
        }
        finally
        {
            _clearOutdatedUserInterfaceModificationSemaphoreSlim.Release();
        }
        
        try
        {
            // Only one UserInterface modification at a time.
            await _executeUserInterfaceModificationSemaphoreSlim.WaitAsync();

            Func<Task>? modificationFunc;
            
            try
            {
                // The current UserModification thread needs to stop anyone from pushing to the Stack
                // otherwise timing errors could result in modifications getting pushed just as we're about to clear
                // causing a lost event.
                await _clearOutdatedUserInterfaceModificationSemaphoreSlim.WaitAsync();
                
                if (!_userInterfaceModificationConcurrentStack.TryPop(out modificationFunc))
                    return;

                // Clear previous events as they are out of date.
                _userInterfaceModificationConcurrentStack.Clear();
            }
            finally
            {
                // Ensure to release so the Stack can be pushed onto again
                // while we await this possibly long running modification.
                _clearOutdatedUserInterfaceModificationSemaphoreSlim.Release();
            }

            // Await the modification func.
            await modificationFunc.Invoke();
        }
        finally
        {
            // Allow a different thread to have a turn at executing a modification func.
            _executeUserInterfaceModificationSemaphoreSlim.Release();
        }
    }
    
    public async Task InitializeVirtualizeIntersectionObserver()
    {
        if (_topBoundaryBlazorComponent is null)
            return;
        
        await JsRuntime.InvokeVoidAsync("virtualizeCoordinateSystem.initializeVirtualizeIntersectionObserver",
            _virtualizeCoordinateSystemIdentifier,
            DotNetObjectReference.Create(this),
            GetVirtualizeBoundaryBlazorComponents()
                .Select(vb => 
                    vb?.Id ?? string.Empty));
    }
    
    [JSInvokable]
    public async Task OnIntersectionObserverThresholdChanged(ScrollDimensions scrollDimensions)
    {
        Console.WriteLine($"OnIntersectionObserverThresholdChanged: {++_onIntersectionThresholdChangedCounter}");

        _scrollDimensions = scrollDimensions;

        // Task.Run as to not block the UI thread when doing the SemaphoreSlim logic in PushUserInterfaceModificationAsync         
        _ = Task.Run(async () =>
        {
            await PushUserInterfaceModificationAsync(async () =>
            {
                Console.WriteLine(
                    $"PushUserInterfaceModificationAsync: {++_pushUserInterfaceModificationAsyncCounter}");

                if (_dimensions is null)
                    return;

                if (_scrollDimensions is null)
                    return;

                var startIndex = _scrollDimensions.ScrollTop / _dimensions.HeightOfItemInPixels;
                var count = _dimensions.HeightOfScrollableContainerInPixels / _dimensions.HeightOfItemInPixels;

                var totalHeight = _dimensions.HeightOfItemInPixels * Items.Count;

                var topBoundaryHeight = _scrollDimensions.ScrollTop;

                var bottomBoundaryHeight =
                    totalHeight - topBoundaryHeight - _dimensions.HeightOfScrollableContainerInPixels;

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

                if (bottomBoundaryHeight < 0)
                {
                    var tooFarAmount = Math.Abs(bottomBoundaryHeight);

                    topBoundaryHeight -= tooFarAmount;
                    bottomBoundaryHeight += tooFarAmount;
                }

                var results = Items
                    .Skip((int)Math.Floor(startIndex))
                    .Take((int)Math.Ceiling(count))
                    .Select((item, i) =>
                        new VirtualizeItemWrapper<TItem>(item,
                            topBoundaryHeight + (i * _dimensions.HeightOfItemInPixels),
                            100))
                    .ToArray();

                _topVirtualizeBoundary.HeightInPixels = topBoundaryHeight;
                _bottomVirtualizeBoundary.HeightInPixels = bottomBoundaryHeight;
                _bottomVirtualizeBoundary.OffsetFromTopInPixels = topBoundaryHeight + heightOfRenderedContent;

                _virtualizeItemWrappers = results;

                await InvokeAsync(StateHasChanged);
            });
        });
    }

    private void OnAfterMeasurementTaken(VirtualizeItemDimensions? virtualizeItemDimensions)
    {
        _forceGetDimensions = false;
        
        _dimensions = virtualizeItemDimensions;
    }

    public void Dispose()
    {
        Task.Run(async () =>
        {
            await JsRuntime.InvokeVoidAsync("virtualizeCoordinateSystem.disposeVirtualizeIntersectionObserver",
                _virtualizeCoordinateSystemIdentifier,
                GetVirtualizeBoundaryBlazorComponents()
                    .Select(vb =>
                        vb?.Id ?? string.Empty));
        });
    }
}