using BlazorStudio.ClassLib.Store.QuickSelectCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.QuickSelect;

public partial class QuickSelectDisplayEntry : ComponentBase
{
    [CascadingParameter]
    public int ActiveEntryIndex { get; set; }

    [Parameter]
    [EditorRequired]
    public IQuickSelectItem QuickSelectItem { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public int EntryIndex { get; set; }

    private string ActiveEntryCssClass => ActiveEntryIndex == EntryIndex
        ? "bstudio_active"
        : string.Empty;
}