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
    public Action<VirtualizeCoordinateSystemMessage> RequestCallbackAction { get; set; } = null!;
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
    /// <summary>
    /// Using this for debugging purposes temporarily
    /// </summary>
    [Parameter]
    public int ForceRerender { get; set; }

    private CancellationTokenSource _cancellationTokenSource = new();

    private Guid _guid;

    private string _leftElementId = $"virtualize-coordinate-system_left_{Guid.NewGuid()}";
    private string _rightElementId = $"virtualize-coordinate-system_right_{Guid.NewGuid()}";
    private string _topElementId = $"virtualize-coordinate-system_top_{Guid.NewGuid()}";
    private string _bottomElementId = $"virtualize-coordinate-system_bottom_{Guid.NewGuid()}";

    private DotNetObjectReference<VirtualizeCoordinateSystem<T>> _dotNetObjectReference = null!;

    private string ComponentId => $"bstudio_virtualize-coordinate-system_{_guid}";

    public VirtualizeCoordinateSystemMessage MostRecentMessage { get; private set; }

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

        RequestCallbackAction(new VirtualizeCoordinateSystemMessage(
            virtualizeCoordinateSystemRequest, null));
    }

    public void SetData(VirtualizeCoordinateSystemMessage message)
    {
        MostRecentMessage = message;
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
    
    private string GetLeftBoundaryCssString(VirtualizeCoordinateSystemRequest? request, VirtualizeCoordinateSystemResult<T>? result)
    {
        if (request is null ||
            result is null)
        {
            return string.Empty;
        }

        var height = Math.Max(result.HeightOfResultInPixels,
            request.ViewportHeightInPixels);

        return $"left: 0px; width: {request.ScrollLeftInPixels}px; height: {height}px;";
    }
    
    private string GetRightBoundaryCssString(VirtualizeCoordinateSystemRequest? request, VirtualizeCoordinateSystemResult<T>? result)
    {
        if (request is null ||
            result is null)
        {
            return string.Empty;
        }

        var contentWidth = Math.Max(result.WidthOfResultInPixels, 
            request.ViewportWidthInPixels);
        
        var leftInPixels = Math.Max(request.ScrollLeftInPixels + contentWidth,
            result.TotalWidthInPixels);

        var boundaryWidthInPixels = Math.Max(result.TotalWidthInPixels - leftInPixels,
            0);

        var height = Math.Max(result.HeightOfResultInPixels,
            request.ViewportHeightInPixels);

        return $"left: {leftInPixels}px;  top: {request.ScrollTopInPixels}px; width: {boundaryWidthInPixels}px; height: {height}px;";
    }

    private string GetTopBoundaryCssString(VirtualizeCoordinateSystemRequest? request, VirtualizeCoordinateSystemResult<T>? result)
    {
        if (request is null ||
            result is null)
        {
            return string.Empty;
        }

        var boundaryHeightInPixels = request.ScrollTopInPixels;

        var boundaryWidthInPixels = Math.Max(result.WidthOfResultInPixels,
            request.ViewportWidthInPixels);

        var leftInPixels = request.ScrollLeftInPixels;

        return $"top: 0px; left: {leftInPixels}px; width: {boundaryWidthInPixels}px; height: {boundaryHeightInPixels}px";
    }

    private string GetBottomBoundaryCssString(VirtualizeCoordinateSystemRequest? request, VirtualizeCoordinateSystemResult<T>? result)
    {
        if (request is null ||
            result is null)
        {
            return string.Empty;
        }

        var contentHeight = Math.Max(result.HeightOfResultInPixels,
            request.ViewportHeightInPixels);

        var topInPixels = request.ScrollTopInPixels
                                     + contentHeight;

        var boundaryHeightInPixels = Math.Max(result.TotalHeightInPixels
                                     - topInPixels
                                     - contentHeight,
            0);

        var leftInPixels = request.ScrollLeftInPixels;

        var contentWidth = Math.Max(result.WidthOfResultInPixels,
            request.ViewportWidthInPixels);

        var boundaryWidthInPixels = Math.Max(result.TotalWidthInPixels 
                                    - leftInPixels 
                                    - contentWidth,
            request.ViewportWidthInPixels);

        return $"top: {topInPixels}px; left: {leftInPixels}px; width: {boundaryWidthInPixels}px; height: {boundaryHeightInPixels}px";
    }
    
    private string GetContentCssString(VirtualizeCoordinateSystemRequest? request, VirtualizeCoordinateSystemResult<T>? result)
    {
        if (request is null ||
            result is null)
        {
            return string.Empty;
        }

        return $"left: {request.ScrollLeftInPixels}px; top: {request.ScrollTopInPixels}px; min-width: 100%; min-height: 100%;";
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