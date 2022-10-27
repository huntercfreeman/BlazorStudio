using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.Virtualization;

public partial class VirtualizationDisplay<T> : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public Func<VirtualizationRequest, VirtualizationResult<T>?> EntriesProviderFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<VirtualizationEntry<T>> ChildContent { get; set; } = null!;

    [Parameter]
    public bool UseHorizontalVirtualization { get; set; } = true;
    [Parameter]
    public bool UseVerticalVirtualization { get; set; } = true;
    
    private ElementReference _scrollableParentFinder;
    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();

    private VirtualizationResult<T> _result = new VirtualizationResult<T>(
        ImmutableArray<VirtualizationEntry<T>>.Empty, 
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0));

    private CancellationTokenSource _scrollEventCancellationTokenSource = new();
    private VirtualizationRequest _request = null!;

    private string LeftVirtualizationBoundaryDisplayId =>
        $"bte_left-virtualization-boundary-display-{_intersectionObserverMapKey}";
    
    private string RightVirtualizationBoundaryDisplayId =>
        $"bte_right-virtualization-boundary-display-{_intersectionObserverMapKey}";
    
    private string TopVirtualizationBoundaryDisplayId =>
        $"bte_top-virtualization-boundary-display-{_intersectionObserverMapKey}";
    
    private string BottomVirtualizationBoundaryDisplayId =>
        $"bte_bottom-virtualization-boundary-display-{_intersectionObserverMapKey}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var boundaryIds = new List<object>();

            if (UseHorizontalVirtualization)
            {
                boundaryIds.AddRange(new []
                {
                    LeftVirtualizationBoundaryDisplayId,
                    RightVirtualizationBoundaryDisplayId
                });
            }
            
            if (UseVerticalVirtualization)
            {
                boundaryIds.AddRange(new []
                {
                    TopVirtualizationBoundaryDisplayId,
                    BottomVirtualizationBoundaryDisplayId
                });
            }
            
            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditorVirtualization.initializeIntersectionObserver",
                _intersectionObserverMapKey.ToString(),
                DotNetObjectReference.Create(this),
                _scrollableParentFinder,
                boundaryIds);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public async Task OnScrollEventAsync(VirtualizationScrollPosition scrollPosition)
    {
        _scrollEventCancellationTokenSource.Cancel();
        _scrollEventCancellationTokenSource = new();

        _request = new VirtualizationRequest(
            scrollPosition,
            _scrollEventCancellationTokenSource.Token);
        
        InvokeEntriesProviderFunc();
    }

    public void InvokeEntriesProviderFunc()
    {
        var localResult = EntriesProviderFunc.Invoke(_request);

        if (localResult is not null)
        {
            _result = localResult;

            InvokeAsync(StateHasChanged);
        }
    }
    
    public void Dispose()
    {
        _scrollEventCancellationTokenSource.Cancel();
        
        _ = Task.Run(async () => 
            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditorVirtualization.disposeIntersectionObserver",
                _intersectionObserverMapKey.ToString()));
    }
}