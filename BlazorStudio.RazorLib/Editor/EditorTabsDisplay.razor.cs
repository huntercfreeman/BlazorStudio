using System.Collections.Immutable;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorTabsDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ImmutableList<TextEditorBase> TextEditorList { get; set; } = null!;
    
    public void ForceReloadTabs()
    {
        InvokeAsync(StateHasChanged);
    }
}