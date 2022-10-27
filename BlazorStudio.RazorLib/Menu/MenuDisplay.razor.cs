using BlazorStudio.ClassLib.Menu;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MenuRecord MenuRecord { get; set; } = null!;
}