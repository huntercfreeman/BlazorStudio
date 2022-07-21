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
    public Action<VirtualizeCoordinateSystemRequest, CancellationToken> RequestCallbackAction { get; set; } = null!;
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
    /// Show a HTML element to help with debugging
    /// </summary>
    [Parameter]
    public bool ShowDebugInfo { get; set; }

    private CancellationTokenSource _cancellationTokenSource = new();
    public ImmutableArray<T> _data = ImmutableArray<T>.Empty;
    public Dimensions _dimensions = null!;
    
    public Dimensions _leftBoundaryDimensions = null!;
    public Dimensions _rightBoundaryDimensions = null!;
    public Dimensions _topBoundaryDimensions = null!;
    public Dimensions _bottomBoundaryDimensions = null!;

    public double _scrollLeft;
    public double _scrollTop;
    public double _javascriptWidthResult;
    public double _javascriptHeightResult;

    public Guid _guid;

    public string _leftElementId = $"virtualize-coordinate-system_left_{Guid.NewGuid()}";
    public string _rightElementId = $"virtualize-coordinate-system_right_{Guid.NewGuid()}";
    public string _topElementId = $"virtualize-coordinate-system_top_{Guid.NewGuid()}";
    public string _bottomElementId = $"virtualize-coordinate-system_bottom_{Guid.NewGuid()}";

    private DotNetObjectReference<VirtualizeCoordinateSystem<T>> _dotNetObjectReference = null!;

    public event EventHandler _componentStateChanged;

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

        

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <param name="id">The id of the specific Boundary that was scrolled into view</param>
    [JSInvokable]
    public void FireRequestCallbackAction(string id, double scrollTop, double scrollLeft, double width, double height)
    {
        _scrollLeft = scrollLeft;
        _scrollTop = scrollTop;
        _javascriptWidthResult = width;
        _javascriptHeightResult = height;

        _componentStateChanged?.Invoke(null, EventArgs.Empty);

        var virtualizeCoordinateSystemRequest = new VirtualizeCoordinateSystemRequest();

        RequestCallbackAction(virtualizeCoordinateSystemRequest, CancelTokenSourceAndGetNewToken());
    }
    
    public void SetData(VirtualizeCoordinateSystemResult<T> virtualizeCoordinateSystemResult)
    {
        _data = virtualizeCoordinateSystemResult.Items.ToImmutableArray();
        _dimensions = virtualizeCoordinateSystemResult.CoordinateSystemDimensions;

        //_leftBoundaryDimensions = InitialVirtualizeCoordinateSystemResult.LeftBoundaryDimensions;
        //_rightBoundaryDimensions = InitialVirtualizeCoordinateSystemResult.RightBoundaryDimensions;
        //_topBoundaryDimensions = InitialVirtualizeCoordinateSystemResult.TopBoundaryDimensions;
        //_bottomBoundaryDimensions = InitialVirtualizeCoordinateSystemResult.BottomBoundaryDimensions;

        InvokeAsync(StateHasChanged);

        _componentStateChanged?.Invoke(null, EventArgs.Empty);
    }
    
    public CancellationToken CancelTokenSourceAndGetNewToken()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        return _cancellationTokenSource.Token;
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