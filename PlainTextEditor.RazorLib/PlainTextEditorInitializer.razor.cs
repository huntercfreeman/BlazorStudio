using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PlainTextEditor.RazorLib;

public partial class PlainTextEditorInitializer : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

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
