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

    private List<NugetPackageManagerTreeViewEntry> GetRootNugetPackageManagerTreeViewEntries()
    {
        return _nugetPackages
            .Select(np => new NugetPackageManagerTreeViewEntry
            {
                Item = np,
                IsExpandable = true,
                TitleDisplay = np.Title,
                NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.NugetPackage
            }).ToList();
    }

    private Task<IEnumerable<NugetPackageManagerTreeViewEntry>> LoadThemesChildren(NugetPackageManagerTreeViewEntry nugetPackageManagerTreeViewEntry)
    {
        var children = new List<NugetPackageManagerTreeViewEntry>();
        
        switch (nugetPackageManagerTreeViewEntry.NugetPackageManagerTreeViewEntryKind)
        {
            case NugetPackageManagerTreeViewEntryKind.StringValue:
                break;
            case NugetPackageManagerTreeViewEntryKind.WrappedStringValue:
                var wrappedStringValue = (WrappedStringValue)nugetPackageManagerTreeViewEntry.Item;

                children.Add(new NugetPackageManagerTreeViewEntry
                    {
                        Item = wrappedStringValue.StringValue,
                        IsExpandable = true,
                        TitleDisplay = wrappedStringValue.StringValue,
                        NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
                    });
                
                break;
            case NugetPackageManagerTreeViewEntryKind.StringValueEnumerable:
                var stringValueEnumerable = (IEnumerable<string>)nugetPackageManagerTreeViewEntry.Item;

                children.AddRange(stringValueEnumerable
                    .Select(x => new NugetPackageManagerTreeViewEntry
                    {
                        Item = x,
                        IsExpandable = false,
                        TitleDisplay = x,
                        NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
                    }));
                
                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersion:
                var nugetPackageVersion = (NugetPackageVersionRecord)nugetPackageManagerTreeViewEntry.Item;

                children.Add(new NugetPackageManagerTreeViewEntry
                {
                    Item = nugetPackageVersion.Downloads,
                    IsExpandable = false,
                    TitleDisplay = $"Downloads: {nugetPackageVersion.Downloads:N0}",
                    NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
                });
                
                children.Add(new NugetPackageManagerTreeViewEntry
                {
                    Item = nugetPackageVersion.Version,
                    IsExpandable = false,
                    TitleDisplay = $"Version: {nugetPackageVersion.Version}",
                    NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
                });
                
                children.Add(new NugetPackageManagerTreeViewEntry
                {
                    Item = nugetPackageVersion.AtId,
                    IsExpandable = false,
                    TitleDisplay = $"@id: {nugetPackageVersion.AtId}",
                    NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
                });
                
                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersionEnumerable:
                var nugetPackageVersionEnumerable = (IEnumerable<NugetPackageVersionRecord>)nugetPackageManagerTreeViewEntry.Item;

                children.AddRange(nugetPackageVersionEnumerable
                    .Select(x => new NugetPackageManagerTreeViewEntry
                    {
                        Item = x,
                        IsExpandable = false,
                        TitleDisplay = x.Version,
                        NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.NugetPackageVersion
                    }));
                
                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackage:
                var nugetPackage = (NugetPackageRecord)nugetPackageManagerTreeViewEntry.Item;

                GetNugetPackageRecordChildren(ref children, nugetPackage);
                
                break;
        }
        
        return Task.FromResult(children.AsEnumerable());
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
    
     private IEnumerable<NugetPackageManagerTreeViewEntry> GetNugetPackageRecordChildren(ref List<NugetPackageManagerTreeViewEntry> children,
        NugetPackageRecord nugetPackageRecord)
    {
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Authors,
            IsExpandable = false,
            TitleDisplay = "Authors",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValueEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = new WrappedStringValue { StringValue = nugetPackageRecord.Description },
            IsExpandable = false,
            TitleDisplay = "Description",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.WrappedStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Id,
            IsExpandable = false,
            TitleDisplay = $"Id: {nugetPackageRecord.Id}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Owners,
            IsExpandable = false,
            TitleDisplay = "Owners",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValueEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Registration,
            IsExpandable = false,
            TitleDisplay = $"Registration: {nugetPackageRecord.Registration}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = new WrappedStringValue { StringValue = nugetPackageRecord.Summary },
            IsExpandable = false,
            TitleDisplay = "Summary",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.WrappedStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Tags,
            IsExpandable = false,
            TitleDisplay = "Tags",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValueEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Title,
            IsExpandable = false,
            TitleDisplay = $"Title: {nugetPackageRecord.Title}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Type,
            IsExpandable = false,
            TitleDisplay = $"Type: {nugetPackageRecord.Type}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Verified,
            IsExpandable = false,
            TitleDisplay = $"Verified: {nugetPackageRecord.Verified}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Version,
            IsExpandable = false,
            TitleDisplay = $"Recent Version: {nugetPackageRecord.Version}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Versions,
            IsExpandable = false,
            TitleDisplay = "Versions",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.NugetPackageVersionEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.AtId,
            IsExpandable = false,
            TitleDisplay = $"@id: {nugetPackageRecord.AtId}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.IconUrl,
            IsExpandable = false,
            TitleDisplay = $"IconUrl: {nugetPackageRecord.IconUrl}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.LicenseUrl,
            IsExpandable = false,
            TitleDisplay = $"LicenseUrl: {nugetPackageRecord.LicenseUrl}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.ProjectUrl,
            IsExpandable = false,
            TitleDisplay = $"ProjectUrl: {nugetPackageRecord.ProjectUrl}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.TotalDownloads,
            IsExpandable = false,
            TitleDisplay = $"Total Downloads: {nugetPackageRecord.TotalDownloads:N0}",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.StringValue
        });

        return children;
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
        WrappedStringValue,
        StringValueEnumerable,
    }
    
    /// <summary>
    /// This class is used when a string's value is very large and should have its own
    /// child to hide the text until desired that it is shown.
    /// </summary>
    private class WrappedStringValue
    {
        public string StringValue { get; set; }
    }
}