using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalTabs : ComponentBase
{
    [Parameter, EditorRequired]
    public ImmutableList<TerminalEntry> TerminalEntries { get; set; } = null!;
}