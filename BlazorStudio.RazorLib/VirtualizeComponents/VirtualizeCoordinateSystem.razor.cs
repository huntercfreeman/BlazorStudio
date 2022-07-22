using System.Collections.Immutable;
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
    public RenderFragment<T> ItemRenderFragment { get; set; } = null!;
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
    [Parameter]
    public string ContentWrapperCssClass { get; set; } = string.Empty;
    /// <summary>
    /// Running into a timing problem and am using this as a temporary fix while developing
    /// </summary>
    [Parameter]
    public int ForceRerender { get; set; }
    /// <summary>
    /// This is an optional RenderFragment that is typically used
    /// to mark the position of an item out of scroll viewport
    ///
    /// Example: One can position: absolute and mark the position of an active element even if it is not rendered.
    ///
    /// In general position: absolute must be used to not break virtualization.
    /// </summary>
    [Parameter]
    public RenderFragment? MarkerRenderFragment { get; set; }
    /// <summary>
    /// Show a HTML element to help with debugging
    /// </summary>
    [Parameter]
    public bool ShowDebugInfo { get; set; }

    private CancellationTokenSource _cancellationTokenSource = new();
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

    private string ComponentId => $"bstudio_virtualize-coordinate-system_{_guid}";

    public VirtualizeCoordinateSystemResult<T> VirtualizeCoordinateSystemResult { get; private set; }
    public ImmutableArray<T> Data { get; private set; } = ImmutableArray<T>.Empty;

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

            await JsRuntime.InvokeVoidAsync("blazorStudio.subscribeVirtualizeCoordinateSystemInsersectionObserver",
                _dotNetObjectReference,
                new string[]
                {
                    _leftElementId,
                    _rightElementId,
                    _topElementId,
                    _bottomElementId,
                });

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
        double scrollLeft, 
        double scrollTop, 
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
        Data = virtualizeCoordinateSystemResult.Items.ToImmutableArray();
        VirtualizeCoordinateSystemResult = virtualizeCoordinateSystemResult;
    }

    public async Task RerenderAsync()
    {
        // The caller of this method is not guaranteed to be on the UI thread.
        await InvokeAsync(StateHasChanged);
    }

    private async void OnScroll()
    {
        await JsRuntime.InvokeVoidAsync("blazorStudio.checkIfInView",
            _dotNetObjectReference,
            new string[]
            {
                _leftElementId,
                _rightElementId,
                _topElementId,
                _bottomElementId,
            });
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

        return $"left: 0px; width: {localVirtualizeCoordinateSystemResult.ScrollLeft}px; height: {localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockHeight}px;";
    }
    
    private string GetRightBoundaryCssString(VirtualizeCoordinateSystemResult<T>? localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        var leftInPixels = Math.Min(localVirtualizeCoordinateSystemResult.ScrollLeft 
                           + localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockWidth,
            localVirtualizeCoordinateSystemResult.ScrollWidth);

        var widthInPixels = localVirtualizeCoordinateSystemResult.ScrollWidth
                            - leftInPixels;

        if (widthInPixels < 0)
        {
            widthInPixels = 0;
        }
        
        if (widthInPixels > 0)
        {
            widthInPixels = localVirtualizeCoordinateSystemResult.ScrollWidth;
        }

        return $"left: {leftInPixels}px;  top: {localVirtualizeCoordinateSystemResult.ScrollTop}px; width: {widthInPixels}px; height: {localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockHeight}px;";
    }

    private string GetTopBoundaryCssString(VirtualizeCoordinateSystemResult<T>? localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        var heightInPixels = localVirtualizeCoordinateSystemResult.ScrollTop;

        return $"top: 0px; left: {localVirtualizeCoordinateSystemResult.ScrollLeft}px; width: {localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockWidth}px; height: {heightInPixels}px";
    }

    private string GetBottomBoundaryCssString(VirtualizeCoordinateSystemResult<T>? localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        var topInPixels = Math.Min(localVirtualizeCoordinateSystemResult.ScrollTop +
                                   localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockHeight,
            localVirtualizeCoordinateSystemResult.ScrollHeight);

        var heightInPixels = Math.Max(localVirtualizeCoordinateSystemResult.ScrollHeight -
                             topInPixels,
            0);

        return $"top: {topInPixels}px; left: {localVirtualizeCoordinateSystemResult.ScrollLeft}px; width: {localVirtualizeCoordinateSystemResult.VirtualizeRenderBlockWidth}px; height: {heightInPixels}px";
    }
    
    private string GetContentCssString(VirtualizeCoordinateSystemResult<T>? localVirtualizeCoordinateSystemResult)
    {
        if (localVirtualizeCoordinateSystemResult is null)
            return string.Empty;

        return $"left: {localVirtualizeCoordinateSystemResult.ScrollLeft}px; top: {localVirtualizeCoordinateSystemResult.ScrollTop}px; min-width: 100%; min-height: 100%;";
    }

    public virtual void Dispose()
    {
        _cancellationTokenSource?.Cancel();

        JsRuntime.InvokeVoidAsync("blazorStudio.disposeVirtualizeCoordinateSystemInsersectionObserver",
            new string[]
            {
                _leftElementId,
                _rightElementId,
                _topElementId,
                _bottomElementId,
            });
    }
}