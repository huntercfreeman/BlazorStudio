using BlazorStudio.ClassLib.Store.MenuCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuOptionDisplay : ComponentBase
{
    [Parameter]
    public MenuOptionRecord MenuOptionRecord { get; set; } = null!;
}