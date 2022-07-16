using BlazorStudio.ClassLib.Store.DialogCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogDisplay : ComponentBase
{
    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;
}