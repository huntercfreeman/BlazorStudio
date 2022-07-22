﻿using System.Collections.Immutable;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.VirtualizeComponents;

public partial class VirtualizeCoordinateSystem<T> : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Used when scrolling and the cached content runs out and different cache needs loading
    /// </summary>
    [Parameter, EditorRequired]
    public Action<VirtualizeCoordinateSystemRequest> RequestCallbackAction { get; set; } = null!;
    /// <summary>
    /// Used to render the Generic Item
    /// </summary>
    [Parameter, EditorRequired]
    public Dimensions DimensionsOfCoordinateSystemViewport { get; set; } = null!;
    /// <summary>
    /// Used to render the Generic Item
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment<T> ChildContent { get; set; } = null!;
    /// <summary>
    /// Load more pixels of context just out of viewport to reduce
    /// use seeing a loading template.
    ///
    /// Padding is applied left, right, top, bottom.
    /// </summary>
    [Parameter, EditorRequired]
    public double PaddingInPixels { get; set; }
    [Parameter]
    public Func<Task>? OnAfterFirstRenderCallbackFunc { get; set; }
    /// <summary>
    /// Show a HTML element to help with debugging
    /// </summary>
    [Parameter]
    public bool ShowDebugInfo { get; set; }

    private CancellationTokenSource _cancellationTokenSource = new();
    private ImmutableArray<T> _data = ImmutableArray<T>.Empty;
    private Dimensions _dimensions = null!;
    
    private Dimensions _leftBoundaryDimensions = null!;
    private Dimensions _rightBoundaryDimensions = null!;
    private Dimensions _topBoundaryDimensions = null!;
    private Dimensions _bottomBoundaryDimensions = null!;

    private VirtualizeCoordinateSystemRequest? _mostRecentvirtualizeCoordinateSystemRequest;

    private Guid _guid;

    private string _leftElementId = $"virtualize-coordinate-system_left_{Guid.NewGuid()}";
    private string _rightElementId = $"virtualize-coordinate-system_right_{Guid.NewGuid()}";
    private string _topElementId = $"virtualize-coordinate-system_top_{Guid.NewGuid()}";
    private string _bottomElementId = $"virtualize-coordinate-system_bottom_{Guid.NewGuid()}";

    private DotNetObjectReference<VirtualizeCoordinateSystem<T>> _dotNetObjectReference = null!;
    private VirtualizeCoordinateSystemResult<T> _virtualizeCoordinateSystemResult;

    private string ComponentId => $"bstudio_virtualize-coordinate-system_{_guid}";

    protected override void OnInitialized()
    {
        _guid = Guid.NewGuid();

        _leftElementId = $"virtualize-coordinate-system_left_{_guid}";
        _rightElementId = $"virtualize-coordinate-system_right_{_guid}";
        _topElementId = $"virtualize-coordinate-system_top_{_guid}";
        _bottomElementId = $"virtualize-coordinate-system_bottom_{_guid}";

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);

            await JsRuntime.InvokeVoidAsync("blazorStudio.subscribeVirtualizeCoordinateSystemScrollIntoView",
                _leftElementId,
                _dotNetObjectReference);

            await JsRuntime.InvokeVoidAsync("blazorStudio.subscribeVirtualizeCoordinateSystemScrollIntoView",
                _rightElementId,
                _dotNetObjectReference);

            await JsRuntime.InvokeVoidAsync("blazorStudio.subscribeVirtualizeCoordinateSystemScrollIntoView",
                _topElementId,
                _dotNetObjectReference);

            await JsRuntime.InvokeVoidAsync("blazorStudio.subscribeVirtualizeCoordinateSystemScrollIntoView",
                _bottomElementId,
                _dotNetObjectReference);

            await JsRuntime.InvokeVoidAsync("blazorStudio.getDimensions",
                ComponentId,
                _dotNetObjectReference);

            if (OnAfterFirstRenderCallbackFunc is not null)
                await OnAfterFirstRenderCallbackFunc.Invoke();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <param name="id">The id of the specific Boundary that was scrolled into view</param>
    [JSInvokable]
    public void FireRequestCallbackAction(string id, 
        double scrollTop, 
        double scrollLeft, 
        double scrollWidth, 
        double scrollHeight, 
        double viewportWidth, 
        double viewportHeight)
    {
        var virtualizeCoordinateSystemRequest = new VirtualizeCoordinateSystemRequest(
            scrollLeft,
            scrollTop,
            scrollWidth,
            scrollHeight,
            viewportWidth,
            viewportHeight,
            CancelTokenSourceAndGetNewToken());

        RequestCallbackAction(virtualizeCoordinateSystemRequest);

        _mostRecentvirtualizeCoordinateSystemRequest = virtualizeCoordinateSystemRequest;
    }

    public void SetData(VirtualizeCoordinateSystemResult<T> virtualizeCoordinateSystemResult)
    {
        _data = virtualizeCoordinateSystemResult.Items.ToImmutableArray();
        _virtualizeCoordinateSystemResult = virtualizeCoordinateSystemResult;
    }

    public async Task RerenderAsync()
    {
        // The caller of this method is not guaranteed to be on the UI thread.
        await InvokeAsync(StateHasChanged);
    }

    public CancellationToken CancelTokenSourceAndGetNewToken()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        return _cancellationTokenSource.Token;
    }
    
    private string GetLeftBoundaryCssString(VirtualizeCoordinateSystemResult<T>? localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        var widthInPixels = Math.Max(localVirtualizeCoordinateSystemResult.ScrollLeft - PaddingInPixels, 
            0);

        return $"left: 0px; width: {widthInPixels}px; height: 100%;";
    }
    
    private string GetRightBoundaryCssString(VirtualizeCoordinateSystemResult<T> localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        var leftInPixels = Math.Min(localVirtualizeCoordinateSystemResult.ScrollLeft + 
                                    localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockWidth,
            localVirtualizeCoordinateSystemResult.ScrollWidth);
        
        var widthInPixels = Math.Max(localVirtualizeCoordinateSystemResult.ScrollWidth -
                                     leftInPixels -
                                     Math.Max(localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockWidth,
                                         localVirtualizeCoordinateSystemResult.ScrollWidth),
            0);

        return $"left: {leftInPixels}px; width: {widthInPixels}px; height: 100%;";
    }

    private string GetTopBoundaryCssString(VirtualizeCoordinateSystemResult<T> localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        var heightInPixels = localVirtualizeCoordinateSystemResult.ScrollTop;

        return $"top: 0px; width: 100%; height: {heightInPixels}px";
    }

    private string GetBottomBoundaryCssString(VirtualizeCoordinateSystemResult<T> localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        var topInPixels = Math.Max(localVirtualizeCoordinateSystemResult.ScrollTop +
                                   localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockHeight,
            localVirtualizeCoordinateSystemResult.ScrollTop);

        var heightInPixels = Math.Min(localVirtualizeCoordinateSystemResult.ScrollTop -
                             topInPixels,
            0);

        return $"top: {topInPixels}px; width: 100%; height: {heightInPixels}px";
    }

    public virtual void Dispose()
    {
        _cancellationTokenSource?.Cancel();

        JsRuntime.InvokeVoidAsync("blazorStudio.disposeVirtualizeCoordinateSystemScrollIntoView",
            _leftElementId);

        JsRuntime.InvokeVoidAsync("blazorStudio.disposeVirtualizeCoordinateSystemScrollIntoView",
            _rightElementId);

        JsRuntime.InvokeVoidAsync("blazorStudio.disposeVirtualizeCoordinateSystemScrollIntoView",
            _topElementId);

        JsRuntime.InvokeVoidAsync("blazorStudio.disposeVirtualizeCoordinateSystemScrollIntoView",
            _bottomElementId);
    }
}