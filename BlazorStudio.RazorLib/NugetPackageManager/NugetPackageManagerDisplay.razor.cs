using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.NugetPackageManager;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.FooterWindowCase;
using BlazorStudio.ClassLib.Store.NugetPackageManagerCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.RazorLib.ContextCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.NugetPackageManager;

public partial class NugetPackageManagerDisplay : FluxorComponent
{
    [Inject]
    private INugetPackageManagerProvider NugetPackageManagerProvider { get; set; } = null!;
    [Inject]
    private IState<NugetPackageManagerState> NugetPackageManagerStateWrapper { get; set; } = null!;
    [Inject]
    private IState<SolutionState> SolutionStateWrapper { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private SequenceKey? _previousFocusRequestedSequenceKey;
    private ContextBoundary? _contextBoundary;
    private string _nugetQuery = string.Empty;
    private bool _includePrerelease;
    
    protected override void OnInitialized()
    {
        NugetPackageManagerStateWrapper.StateChanged += NugetPackageManagerStateWrapperOnStateChanged;
        
        base.OnInitialized();
    }

    private async void NugetPackageManagerStateWrapperOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousFocusRequestedSequenceKey is null ||
            _previousFocusRequestedSequenceKey != NugetPackageManagerStateWrapper.Value.FocusRequestedSequenceKey)
        {
            if (_contextBoundary is not null)
            {
                await _contextBoundary.HandleOnFocusInAsync(null);
                ContextFacts.NugetPackageManagerDisplayContext.InvokeOnFocusRequestedEventHandler();
            }
        }
    }
    
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        var keyDownEventRecord = new KeyDownEventRecord(
            keyboardEventArgs.Key,
            keyboardEventArgs.Code,
            keyboardEventArgs.CtrlKey,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.AltKey);
        
        if (keyDownEventRecord.Code == KeyboardKeyFacts.NewLineCodes.ENTER_CODE)
        {
            // TODO: Send http request
        }
    }

    protected override void Dispose(bool disposing)
    {
        NugetPackageManagerStateWrapper.StateChanged -= NugetPackageManagerStateWrapperOnStateChanged;

        base.Dispose(disposing);
    }
}