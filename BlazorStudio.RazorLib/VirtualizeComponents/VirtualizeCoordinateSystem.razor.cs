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
    public RenderFragment<T> ChildContent { get; set; } = null!;
    /// <summary>
    /// Load more pixels of context just out of viewport to reduce
    /// use seeing a loading template.
    ///
    /// Padding is applied left, right, top, bottom.
    /// </summary>
    [Parameter, EditorRequired]
    public double PaddingInPixels { get; set; }
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

    public double ScrollTop { get; private set; }
    public double ScrollLeft { get; private set; }
    public double ScrollWidth { get; private set; }
    public double ScrollHeight { get; private set; }
    public double ViewportWidth { get; private set; }
    public double ViewportHeight { get; private set; }

    private Guid _guid;

    private string _leftElementId = $"virtualize-coordinate-system_left_{Guid.NewGuid()}";
    private string _rightElementId = $"virtualize-coordinate-system_right_{Guid.NewGuid()}";
    private string _topElementId = $"virtualize-coordinate-system_top_{Guid.NewGuid()}";
    private string _bottomElementId = $"virtualize-coordinate-system_bottom_{Guid.NewGuid()}";

    private DotNetObjectReference<VirtualizeCoordinateSystem<T>> _dotNetObjectReference = null!;
    
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
        ScrollLeft = scrollLeft;
        ScrollTop = scrollTop;
        ScrollWidth = scrollWidth;
        ScrollHeight = scrollHeight;
        ViewportWidth = viewportWidth;
        ViewportHeight = viewportHeight;

        var virtualizeCoordinateSystemRequest = new VirtualizeCoordinateSystemRequest(
            ScrollLeft,
            ScrollTop,
            ScrollWidth,
            ScrollHeight,
            ViewportWidth,
            ViewportHeight,
            CancelTokenSourceAndGetNewToken());

        RequestCallbackAction(virtualizeCoordinateSystemRequest);
    }
    
    public void SetData(VirtualizeCoordinateSystemResult<T> virtualizeCoordinateSystemResult)
    {
        _data = virtualizeCoordinateSystemResult.Items.ToImmutableArray();
        _dimensions = virtualizeCoordinateSystemResult.CoordinateSystemViewportDimensions;

        InvokeAsync(StateHasChanged);
    }
    
    public CancellationToken CancelTokenSourceAndGetNewToken()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        return _cancellationTokenSource.Token;
    }
    
    private string GetLeftBoundaryCssString()
    {
        return $"";
    }
    
    private string GetRightBoundaryCssString()
    {
        return $"";
    }

    private string GetTopBoundaryCssString()
    {
        return $"";
    }

    private string GetBottomBoundaryCssString()
    {
        return $"";
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