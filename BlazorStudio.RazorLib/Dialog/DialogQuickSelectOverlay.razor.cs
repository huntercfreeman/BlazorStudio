using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.RazorLib.ContextCase;
using BlazorStudio.RazorLib.Focus;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogQuickSelectOverlay : FluxorComponent
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;

    private ContextBoundary _contextBoundary;
    private bool _displayDialogQuickSelectOverlay;
    private int _activeEntryIndex;
    private FocusTrap? _focusTrap;
    private DialogKey? _previouslyActiveDialogKey;

    protected override void OnInitialized()
    {
        ContextStateSelector.Select(x => x.ContextRecords[ContextFacts.DialogDisplayContext.ContextKey]);

        // Set starting _activeEntryIndex 
        {
            _activeEntryIndex = 1;

            if (DialogStatesWrap.Value.List.Count < 1)
            {
                _activeEntryIndex = 0;
            }
        }

        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            ContextStateSelector.Value.OnFocusRequestedEventHandler += ValueOnOnFocusRequestedEventHandler;
        }

        if (_activeEntryIndex > DialogStatesWrap.Value.List.Count - 1)
        {
            _activeEntryIndex = 0;
        }
        
        base.OnAfterRender(firstRender);
    }

    private async void ValueOnOnFocusRequestedEventHandler(object? sender, EventArgs e)
    {
        if (!_displayDialogQuickSelectOverlay)
        {
            _displayDialogQuickSelectOverlay = true;
        }
        else
        {
            if (_activeEntryIndex < DialogStatesWrap.Value.List.Count - 1)
            {
                _activeEntryIndex++;
            }
            else
            {
                _activeEntryIndex = 0;
            }
        }

        if (DialogStatesWrap.Value.List.Count == 0)
        {
            if (_focusTrap is not null)
                await _focusTrap.FocusAsync();
        }

        await InvokeAsync(StateHasChanged);
    }
    
    private async Task HandleOnKeyUpAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.AltKey == false)
        {
            _displayDialogQuickSelectOverlay = false;
            
            var dialogStates = DialogStatesWrap.Value;

            if (_activeEntryIndex < dialogStates.List.Count - 1)
            {
                var activeDialog = dialogStates.List[_activeEntryIndex];
            
                activeDialog.InvokeOnFocusRequestedEventHandler();    
            }
            else
            {
                _displayDialogQuickSelectOverlay = false;
                await InvokeAsync(StateHasChanged);
            }
            
            // Set starting _activeEntryIndex 
            {
                _activeEntryIndex = 1;

                if (DialogStatesWrap.Value.List.Count < 1)
                {
                    _activeEntryIndex = 0;
                }
            }
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        ContextStateSelector.Value.OnFocusRequestedEventHandler -= ValueOnOnFocusRequestedEventHandler;
    }
}