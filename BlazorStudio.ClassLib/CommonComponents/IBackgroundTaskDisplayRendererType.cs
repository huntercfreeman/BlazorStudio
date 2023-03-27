using BlazorStudio.ClassLib.BackgroundTaskCase;

namespace BlazorStudio.ClassLib.CommonComponents;

public interface IBackgroundTaskDisplayRendererType
{
    public IBackgroundTask BackgroundTask { get; set; }
}