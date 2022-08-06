using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalEntryDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TerminalEntryOutputStates, string> TerminalEntryOutputStatesSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public TerminalEntry TerminalEntry { get; set; } = null!;

    protected override void OnInitialized()
    {
        TerminalEntryOutputStatesSelection
            .Select(x =>
            {
                if (!x.OutputMap.TryGetValue(TerminalEntry.TerminalEntryKey, out var output))
                {
                    output = string.Empty;
                }

                return output;
            });

        base.OnInitialized();
    }
}