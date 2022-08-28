using BlazorStudio.ClassLib.TextEditor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorDisplay : FluxorComponent
{
    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;
}