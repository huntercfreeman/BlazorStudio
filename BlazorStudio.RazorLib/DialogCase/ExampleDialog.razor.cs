using BlazorStudio.ClassLib.Store.DialogCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.DialogCase;

public partial class ExampleDialog : ComponentBase
{
    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter]
    public string Message { get; set; } = null!;
}