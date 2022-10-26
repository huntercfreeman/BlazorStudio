using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.ContextCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.KeymapCase;

public class KeymapEventListener : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            JsRuntime.InvokeVoidAsync("blazorStudio.initKeymap",
                DotNetObjectReference.Create(this));
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public void DispatchHandleKeymapEvent(KeyDownEventRecord keyDownEventRecord)
    {
        Dispatcher.Dispatch(new KeymapEventAction(keyDownEventRecord, null, CancellationToken.None));
    }
}