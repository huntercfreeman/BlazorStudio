using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorCascadingValues : ComponentBase
{
    [Parameter, EditorRequired]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;
}