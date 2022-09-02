using BlazorStudio.ClassLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorDebugInfo : ComponentBase
{
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextPartition TextPartition { get; set; } = null!;
}