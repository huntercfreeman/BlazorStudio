using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalTabDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public TerminalEntry TerminalEntry { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Index { get; set; }

    private string IsActiveCssClass => TerminalStateWrap.Value.ActiveIndex == Index
        ? "bstudio_active"
        : string.Empty;

    private void DispatchSetActiveTerminalEntryAction()
    {
        Dispatcher.Dispatch(new SetActiveTerminalEntryAction(Index));
    }
    
    private void TerminalTabHandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.NewLineCodes.ENTER_CODE == keyboardEventArgs.Code ||
            KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE == keyboardEventArgs.Code)
        {
            DispatchSetActiveTerminalEntryAction();
        }
    }
}