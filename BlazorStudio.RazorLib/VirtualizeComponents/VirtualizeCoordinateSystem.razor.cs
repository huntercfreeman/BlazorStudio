using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.VirtualizeComponents;

public partial class VirtualizeCoordinateSystem<T> : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public RenderFragment<VirtualizeCoordinateSystemEntry<T>> ChildContent { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<VirtualizeCoordinateSystemScrollPosition, VirtualizeCoordinateSystemResult<T>> ItemsProviderFunc { get; set; } = null!;

    private Guid _intersectionObserverMapKey = Guid.NewGuid();
    
    private CancellationTokenSource _cancellationTokenSource = new();

    private VirtualizeCoordinateSystemResult<T>? _virtualizeCoordinateSystemResult;

    private VirtualizeCoordinateSystemScrollPosition _mostRecentVirtualizeCoordinateSystemScrollPosition  = new(
        0, 
        0, 
        CancellationToken.None);

    private VirtualizeCoordinateSystemBoundary _virtualizeCoordinateSystemBoundaryLeft = null!;
    private VirtualizeCoordinateSystemBoundary _virtualizeCoordinateSystemBoundaryBottom = null!;
    private VirtualizeCoordinateSystemBoundary _virtualizeCoordinateSystemBoundaryTop = null!;
    private VirtualizeCoordinateSystemBoundary _virtualizeCoordinateSystemBoundaryRight = null!;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync(
                "virtualizeCoordinateSystem.initializeVirtualizeIntersectionObserver",
                _virtualizeCoordinateSystemBoundaryLeft.VirtualizeCoordinateSystemBoundaryElementReference,
                _virtualizeCoordinateSystemBoundaryBottom.VirtualizeCoordinateSystemBoundaryElementReference,
                _virtualizeCoordinateSystemBoundaryTop.VirtualizeCoordinateSystemBoundaryElementReference,
                _virtualizeCoordinateSystemBoundaryRight.VirtualizeCoordinateSystemBoundaryElementReference,
                DotNetObjectReference.Create(this),
                _intersectionObserverMapKey);
            
            ReloadItems();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
    
    [JSInvokable]
    public void OnIntersectionObserverThresholdChanged(double scrollLeft, double scrollTop)
    {
        Console.WriteLine($"scrollLeft: {scrollLeft} | scrollTop: {scrollTop}");
        
        _mostRecentVirtualizeCoordinateSystemScrollPosition = new(
            scrollLeft,
            scrollTop,
            CancelAndReturnNewToken());

        ReloadItems();
        
    }

    public void ReloadItems()
    {
        _virtualizeCoordinateSystemResult = ItemsProviderFunc
            .Invoke(_mostRecentVirtualizeCoordinateSystemScrollPosition);
        
        InvokeAsync(StateHasChanged);
    }
    
    private CancellationToken CancelAndReturnNewToken()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        return _cancellationTokenSource.Token;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();

        Task.Run(async () =>
        {
            await JsRuntime.InvokeVoidAsync(
                "virtualizeCoordinateSystem.disposeVirtualizeIntersectionObserver",
                _intersectionObserverMapKey);    
        });
    }
}