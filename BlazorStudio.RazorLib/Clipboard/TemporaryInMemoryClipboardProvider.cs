using BlazorStudio.ClassLib.Clipboard;

namespace BlazorStudio.RazorLib.Clipboard;

/// <summary>
/// I cannot figure out how to get access to operating system clipboard
/// in a Photino application. This will be used until I find out how to get access.
/// </summary>
public class TemporaryInMemoryClipboardProvider : IClipboardProvider
{
    private string _clipboard = string.Empty;
    public Task<string> ReadClipboard()
    {
        return Task.FromResult(_clipboard);
    }

    public Task SetClipboard(string value)
    {
        _clipboard = value;
        return Task.CompletedTask;
    }
}