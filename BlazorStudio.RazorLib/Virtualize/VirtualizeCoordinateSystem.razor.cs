using System.Collections.Immutable;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Virtualize;

public partial class VirtualizeCoordinateSystem<T> : ComponentBase, IDisposable
{
    /// <summary>
    /// Used when scrolling and the cached content runs out and different cache needs loading
    /// </summary>
    [Parameter, EditorRequired]
    public Action<VirtualizeCoordinateSystemRequest, CancellationToken> RequestCallbackAction { get; set; } = null!;
    /// <summary>
    /// Used to render the Generic Item
    /// </summary>
    [Parameter, EditorRequired]
    public VirtualizeCoordinateSystemResult<T> InitialVirtualizeCoordinateSystemResult { get; set; } = null!;
    /// <summary>
    /// Used to render the Generic Item
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment<T> ItemRenderFragment { get; set; } = null!;

    private CancellationTokenSource _cancellationTokenSource = new();
    private ImmutableArray<T> _data = ImmutableArray<T>.Empty;
    private int _totalItemCount;

    public void FireRequestCallbackAction()
    {
        var virtualizeCoordinateSystemRequest = new VirtualizeCoordinateSystemRequest();

        RequestCallbackAction(virtualizeCoordinateSystemRequest, CancelTokenSourceAndGetNewToken());
    }
    
    public void SetData(VirtualizeCoordinateSystemResult<T> virtualizeCoordinateSystemResult)
    {
        _data = virtualizeCoordinateSystemResult.Items.ToImmutableArray();
        _totalItemCount = virtualizeCoordinateSystemResult.TotalItemCount;

        InvokeAsync(StateHasChanged);
    }
    
    public CancellationToken CancelTokenSourceAndGetNewToken()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        return _cancellationTokenSource.Token;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }
}