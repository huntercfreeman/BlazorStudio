using BlazorStudio.ClassLib.BackgroundTaskCase;
using BlazorStudio.ClassLib.CommonComponents;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BackgroundTaskCase;

public partial class BackgroundTaskDisplay : ComponentBase, IBackgroundTaskDisplayRendererType
{
    [Parameter, EditorRequired]
    public IBackgroundTask BackgroundTask { get; set; } = null!;
}