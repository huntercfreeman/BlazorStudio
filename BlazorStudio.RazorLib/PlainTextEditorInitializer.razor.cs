using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib;

public partial class PlainTextEditorInitializer : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public bool InitializeFluxor { get; set; } = true;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            JsRuntime.InvokeVoidAsync("plainTextEditor.initializeIntersectionObserver");
        }

        return base.OnAfterRenderAsync(firstRender);
    }
    
    public void Dispose()
    {
        JsRuntime.InvokeVoidAsync("plainTextEditor.disposeIntersectionObserver");
    }
}
