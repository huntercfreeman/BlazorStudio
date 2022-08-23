using System.Collections.Immutable;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.NugetPackageManager;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.FooterWindowCase;
using BlazorStudio.ClassLib.Store.NugetPackageManagerCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.RazorLib.ContextCase;
using BlazorStudio.RazorLib.TreeViewCase;
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
    private ImmutableArray<NugetPackageRecord> _nugetPackages = ImmutableArray<NugetPackageRecord>.Empty;
    
    private TreeViewWrapKey _nugetPackageManagerTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private List<NugetPackageManagerTreeViewEntry> _rootNugetPackageManagerTreeViewEntries = GetRootNugetPackageManagerTreeViewEntries();
    private string? _activeQueryForNugetPackages;

    private NugetPackageManagerQuery BuiltNugetQuery => NugetPackageManagerProvider
        .BuildQuery(_nugetQuery, _includePrerelease);
    
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
    
    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        // Do not accidentally send many requests
        if (keyboardEventArgs.Repeat)
            return;
        
        var keyDownEventRecord = new KeyDownEventRecord(
            keyboardEventArgs.Key,
            keyboardEventArgs.Code,
            keyboardEventArgs.CtrlKey,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.AltKey);
        
        if (keyDownEventRecord.Code == KeyboardKeyFacts.NewLineCodes.ENTER_CODE)
        {
            try
            {
                // Update UserInterface
                {
                    _activeQueryForNugetPackages = BuiltNugetQuery.Query;
                    await InvokeAsync(StateHasChanged);
                }

                // Perform Query
                _nugetPackages = await NugetPackageManagerProvider.QueryForNugetPackagesAsync(BuiltNugetQuery);
            }
            finally
            {
                // Update UserInterface
                {
                    _activeQueryForNugetPackages = null;
                    // EventCallback will cause rerender no StateHasChanged needed
                }
            }
        }
    }

    private static List<NugetPackageManagerTreeViewEntry> GetRootNugetPackageManagerTreeViewEntries()
    {
    }

    private Task<IEnumerable<NugetPackageManagerTreeViewEntry>> LoadThemesChildren(NugetPackageManagerTreeViewEntry nugetPackageManagerTreeViewEntry)
    {
        switch (nugetPackageManagerTreeViewEntry.NugetPackageManagerTreeViewEntryKind)
        {
            case NugetPackageManagerTreeViewEntryKind.StringValue:
                break;
            case NugetPackageManagerTreeViewEntryKind.StringValueEnumerable:
                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersion:
                return Task.FromResult(
                    Array
                        .Empty<NugetPackageManagerTreeViewEntry>()
                        .AsEnumerable());
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersionEnumerable:
                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackage:
                var nugetPackage = (NugetPackageRecord)nugetPackageManagerTreeViewEntry.Item;
                return Task.FromResult(
                    nugetPackage.)
        }
    }

    private void ThemeTreeViewOnEnterKeyDown(TreeViewKeyboardEventDto<NugetPackageManagerTreeViewEntry> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void ThemeTreeViewOnSpaceKeyDown(TreeViewKeyboardEventDto<NugetPackageManagerTreeViewEntry> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void ThemeTreeViewOnDoubleClick(TreeViewMouseEventDto<NugetPackageManagerTreeViewEntry> treeViewMouseEventDto)
    {
        treeViewMouseEventDto.ToggleIsExpanded.Invoke();
    }

    protected override void Dispose(bool disposing)
    {
        NugetPackageManagerStateWrapper.StateChanged -= NugetPackageManagerStateWrapperOnStateChanged;

        base.Dispose(disposing);
    }

    private class NugetPackageManagerTreeViewEntry
    {
        public NugetPackageManagerTreeViewEntryKind NugetPackageManagerTreeViewEntryKind { get; set; }
        public object Item { get; set; } = null!;
        public string TitleDisplay { get; set; }
        public bool IsExpandable { get; set; }
    }
    
    
    private enum NugetPackageManagerTreeViewEntryKind
    {
        NugetPackage,
        NugetPackageVersion,
        NugetPackageVersionEnumerable,
        StringValue,
        StringValueEnumerable,
    }
}