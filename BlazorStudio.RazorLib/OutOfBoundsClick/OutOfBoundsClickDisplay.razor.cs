using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.OutOfBoundsClick;

public partial class OutOfBoundsClickDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public int ZIndex { get; set; }
    [Parameter, EditorRequired]
    public Action<MouseEventArgs> OnClickCallback { get; set; } = null!;
}