using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.OutOfBoundsClick;

public partial class OutOfBoundsClickDisplay : ComponentBase
{
    [Parameter]
    public Action<MouseEventArgs> ClickedOutOfBounds { get; set; } = null!;
    [Parameter]
    public int ZIndex { get; set; }

    private void ClickedOutOfBoundsOnClick(MouseEventArgs mouseEventArgs)
    {
        ClickedOutOfBounds.Invoke(mouseEventArgs);
    }
}