using BlazorStudio.ClassLib.Store.TerminalCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalEntryDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TerminalEntry TerminalEntry { get; set; } = null!;
}