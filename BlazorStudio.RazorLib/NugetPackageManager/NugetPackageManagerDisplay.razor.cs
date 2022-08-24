using System.Collections.Immutable;
using System.Text.Json;
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
using Microsoft.CodeAnalysis;

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

    private Project? _selectedProjectToModify;
    
    private TreeViewWrapKey _nugetPackageManagerTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrapDisplay<NugetPackageManagerTreeViewEntry>? _nugetPackagesTreeViewWrapDisplay;
    private string? _activeQueryForNugetPackages;
    private JsonException? _jsonExceptionFromQueryingNuget;

    private INugetPackageManagerQuery BuiltNugetQuery => NugetPackageManagerProvider
        .BuildQuery(_nugetQuery, _includePrerelease);
    
    protected override void OnInitialized()
    {
        NugetPackageManagerStateWrapper.StateChanged += NugetPackageManagerStateWrapperOnStateChanged;
        SolutionStateWrapper.StateChanged += SolutionStateWrapperOnStateChanged;
        
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        // Initially select a Project
        SolutionStateWrapperOnStateChanged(null, EventArgs.Empty);
        
        base.OnAfterRender(firstRender);
    }

    private void SolutionStateWrapperOnStateChanged(object? sender, EventArgs e)
    {
        var localSolutionState = SolutionStateWrapper.Value;
        var localSelectedProjectToModify = _selectedProjectToModify;
        
        if (localSelectedProjectToModify is null || 
            !localSolutionState.ProjectIdToProjectMap.TryGetValue(localSelectedProjectToModify.Id, out var project))
        {
            if (localSolutionState.ProjectIdToProjectMap.Any())
            {
                _selectedProjectToModify = 
                    localSolutionState.ProjectIdToProjectMap.Values.ElementAt(0).Project;
            }
        }
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
                    _nugetPackages = ImmutableArray<NugetPackageRecord>.Empty;
                    if (_nugetPackagesTreeViewWrapDisplay is not null)
                    {
                        await InvokeAsync(StateHasChanged);
                        _nugetPackagesTreeViewWrapDisplay.Reload();
                    }
                    _activeQueryForNugetPackages = BuiltNugetQuery.Query;
                    _jsonExceptionFromQueryingNuget = null;
                    await InvokeAsync(StateHasChanged);
                }

                // Perform Query
                try
                {
                    _nugetPackages = await NugetPackageManagerProvider.QueryForNugetPackagesAsync(BuiltNugetQuery);
                    if (_nugetPackagesTreeViewWrapDisplay is not null)
                    {
                        await InvokeAsync(StateHasChanged);
                        _nugetPackagesTreeViewWrapDisplay.Reload();
                    }
                }
                catch (System.Text.Json.JsonException jsonException)
                {
                    _jsonExceptionFromQueryingNuget = jsonException;
                }
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
    
    private void SelectedProjectToModifyChanged(ChangeEventArgs e)
    {
        if (e.Value is not null)
        {
            if (SolutionStateWrapper.Value.ProjectIdToProjectMap
                .TryGetValue(ProjectId.CreateFromSerialized(Guid.Parse((string)e.Value)), out var indexedProject))
            {
                _selectedProjectToModify = indexedProject.Project;
            }
            else
            {
                _selectedProjectToModify = null;
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
                MarkupStringTitleDisplay = (MarkupString)np.Title,
                NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.NugetPackage
            }).ToList();
    }

    private Task<IEnumerable<NugetPackageManagerTreeViewEntry>> LoadThemesChildren(NugetPackageManagerTreeViewEntry nugetPackageManagerTreeViewEntry)
    {
        var children = new List<NugetPackageManagerTreeViewEntry>();
        
        switch (nugetPackageManagerTreeViewEntry.NugetPackageManagerTreeViewEntryKind)
        {
            case NugetPackageManagerTreeViewEntryKind.MarkupStringValue:
                break;
            case NugetPackageManagerTreeViewEntryKind.WrappedMarkupStringValue:
                var wrappedMarkupStringValue = (WrappedMarkupStringValue)nugetPackageManagerTreeViewEntry.Item;

                children.Add(new NugetPackageManagerTreeViewEntry
                    {
                        Item = wrappedMarkupStringValue.MarkupStringValue,
                        IsExpandable = true,
                        MarkupStringTitleDisplay = wrappedMarkupStringValue.MarkupStringValue,
                        NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
                    });
                
                break;
            case NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable:
                var stringValueEnumerable = (IEnumerable<string>)nugetPackageManagerTreeViewEntry.Item;

                children.AddRange(stringValueEnumerable
                    .Select(x => new NugetPackageManagerTreeViewEntry
                    {
                        Item = x,
                        IsExpandable = true,
                        MarkupStringTitleDisplay = (MarkupString)x,
                        NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
                    }));
                
                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersion:
                var nugetPackageVersion = (NugetPackageVersionRecord)nugetPackageManagerTreeViewEntry.Item;

                children.Add(new NugetPackageManagerTreeViewEntry
                {
                    Item = nugetPackageVersion.Downloads,
                    IsExpandable = false,
                    MarkupStringTitleDisplay = (MarkupString)$"Downloads: <em>{nugetPackageVersion.Downloads:N0}</em>",
                    NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
                });
                
                children.Add(new NugetPackageManagerTreeViewEntry
                {
                    Item = nugetPackageVersion.Version,
                    IsExpandable = false,
                    MarkupStringTitleDisplay = (MarkupString)$"Version: <em>{nugetPackageVersion.Version}</em>",
                    NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
                });
                
                children.Add(new NugetPackageManagerTreeViewEntry
                {
                    Item = nugetPackageVersion.AtId,
                    IsExpandable = false,
                    MarkupStringTitleDisplay = (MarkupString)$"@id: <em>{nugetPackageVersion.AtId}</em>",
                    NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
                });
                
                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersionEnumerable:
                var nugetPackageVersionEnumerable = (IEnumerable<NugetPackageVersionRecord>)nugetPackageManagerTreeViewEntry.Item;

                children.AddRange(nugetPackageVersionEnumerable
                    .Select(x => new NugetPackageManagerTreeViewEntry
                    {
                        Item = x,
                        IsExpandable = true,
                        MarkupStringTitleDisplay = (MarkupString)x.Version,
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
    
     private IEnumerable<NugetPackageManagerTreeViewEntry> GetNugetPackageRecordChildren(ref List<NugetPackageManagerTreeViewEntry> children,
        NugetPackageRecord nugetPackageRecord)
    {
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Title,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"Title: <em>{nugetPackageRecord.Title}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.TotalDownloads,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"Total Downloads: <em>{nugetPackageRecord.TotalDownloads:N0}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Version,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"Recent Version: <em>{nugetPackageRecord.Version}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Versions.OrderByDescending(x => x.Version),
            IsExpandable = true,
            MarkupStringTitleDisplay = (MarkupString)"Versions",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.NugetPackageVersionEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Authors,
            IsExpandable = true,
            MarkupStringTitleDisplay = (MarkupString)"Authors",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = new WrappedMarkupStringValue { MarkupStringValue = (MarkupString)nugetPackageRecord.Description },
            IsExpandable = true,
            MarkupStringTitleDisplay = (MarkupString)"Description",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.WrappedMarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Id,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"Id: <em>{nugetPackageRecord.Id}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Owners,
            IsExpandable = true,
            MarkupStringTitleDisplay = (MarkupString)"Owners",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Registration,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"Registration: <em>{nugetPackageRecord.Registration}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = new WrappedMarkupStringValue { MarkupStringValue = (MarkupString)nugetPackageRecord.Summary },
            IsExpandable = true,
            MarkupStringTitleDisplay = (MarkupString)"Summary",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.WrappedMarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Tags,
            IsExpandable = true,
            MarkupStringTitleDisplay = (MarkupString)"Tags",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Type,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"Type: <em>{nugetPackageRecord.Type}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.Verified,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"Verified: <em>{nugetPackageRecord.Verified}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.AtId,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"@id: <em>{nugetPackageRecord.AtId}</em>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.IconUrl,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"IconUrl: <a href='{nugetPackageRecord.IconUrl}'>{nugetPackageRecord.IconUrl}</a>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.LicenseUrl,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"LicenseUrl: <a href='{nugetPackageRecord.LicenseUrl}'>{nugetPackageRecord.LicenseUrl}</a>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });
        
        children.Add(new NugetPackageManagerTreeViewEntry
        {
            Item = nugetPackageRecord.ProjectUrl,
            IsExpandable = false,
            MarkupStringTitleDisplay = (MarkupString)$"ProjectUrl: <a href='{nugetPackageRecord.ProjectUrl}'>{nugetPackageRecord.ProjectUrl}</a>",
            NugetPackageManagerTreeViewEntryKind = NugetPackageManagerTreeViewEntryKind.MarkupStringValue
        });

        return children;
    }
     
    protected override void Dispose(bool disposing)
    {
        NugetPackageManagerStateWrapper.StateChanged -= NugetPackageManagerStateWrapperOnStateChanged;
        SolutionStateWrapper.StateChanged -= SolutionStateWrapperOnStateChanged;

        base.Dispose(disposing);
    }

    public class NugetPackageManagerTreeViewEntry
    {
        public NugetPackageManagerTreeViewEntryKind NugetPackageManagerTreeViewEntryKind { get; set; }
        public object Item { get; set; } = null!;
        public MarkupString MarkupStringTitleDisplay { get; set; }
        public bool IsExpandable { get; set; }
    }
    
    
    public enum NugetPackageManagerTreeViewEntryKind
    {
        NugetPackage,
        NugetPackageVersion,
        NugetPackageVersionEnumerable,
        MarkupStringValue,
        WrappedMarkupStringValue,
        MarkupStringValueEnumerable,
    }
    
    /// <summary>
    /// This class is used when a string's value is very large and should have its own
    /// child to hide the text until desired that it is shown.
    /// </summary>
    public class WrappedMarkupStringValue
    {
        public MarkupString MarkupStringValue { get; set; }
    }
}