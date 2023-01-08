using BlazorStudio.ClassLib.Panel;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Panel;

public partial class PanelTabDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public PanelTab PanelTab { get; set; } = null!;
}