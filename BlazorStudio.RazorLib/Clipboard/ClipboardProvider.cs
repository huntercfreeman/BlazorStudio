using BlazorStudio.ClassLib.Clipboard;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.Clipboard;

public class ClipboardProvider : IClipboardProvider
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> ReadClipboard()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>(
                "plainTextEditor.readClipboard");
        }
        catch (TaskCanceledException e)
        {
            return string.Empty;
        }
    }

    public async Task SetClipboard(string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "plainTextEditor.setClipboard",
                value);
        }
        catch (TaskCanceledException e)
        {
        }
    }
}