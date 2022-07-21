using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Virtualize;

public partial class VirtualizeCoordinateSystem<T> : ComponentBase, IDisposable
{
    /// <summary>
    /// Used when scrolling and the cached content runs out and different cache needs loading
    /// </summary>
    [Parameter, EditorRequired]
    public Action<CancellationToken> RequestCallbackAction { get; set; } = null!;

    private CancellationTokenSource _cancellationTokenSource = new();
    private ImmutableArray<T> _data = ImmutableArray<T>.Empty;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    public void FireRequestCallbackAction()
    {
        RequestCallbackAction(CancelTokenSourceAndGetNewToken());
    }
    
    public void SetData(IEnumerable<T> data)
    {
        _data = data.ToImmutableArray();

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