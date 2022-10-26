using System.Collections.Immutable;
using System.Text.Json;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.NugetPackageManager;
using BlazorStudio.ClassLib.Sequence;
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
    public enum NugetPackageManagerTreeViewEntryKind
    {
        NugetPackage,
        NugetPackageVersion,
        NugetPackageVersionEnumerable,
        MarkupStringValue,
        WrappedMarkupStringValue,
        MarkupStringValueEnumerable,
    }

    private string? _activeQueryForNugetPackages;
    private ContextBoundary? _contextBoundary;
    private bool _includePrerelease;
    private JsonException? _jsonExceptionFromQueryingNuget;

    private TreeViewWrapKey _nugetPackageManagerTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private ImmutableArray<NugetPackageRecord> _nugetPackages = ImmutableArray<NugetPackageRecord>.Empty;
    private TreeViewWrapDisplay<NugetPackageManagerTreeViewEntry>? _nugetPackagesTreeViewWrapDisplay;
    private string _nugetQuery = string.Empty;

    private SequenceKey? _previousFocusRequestedSequenceKey;

    private Project? _selectedProjectToModify;
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
            !localSolutionState.ProjectIdToProjectMap.ContainsKey(localSelectedProjectToModify.Id))
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

        if (keyDownEventRecord.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
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
                catch (JsonException jsonException)
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
                _selectedProjectToModify = indexedProject.Project;
            else
                _selectedProjectToModify = null;
        }
    }

    private List<NugetPackageManagerTreeViewEntry> GetRootNugetPackageManagerTreeViewEntries()
    {
        return _nugetPackages
            .Select(np => new NugetPackageManagerTreeViewEntry(
                np,
                true,
                (MarkupString)np.Title,
                NugetPackageManagerTreeViewEntryKind.NugetPackage)
            ).ToList();
    }

    private Task<IEnumerable<NugetPackageManagerTreeViewEntry>> LoadThemesChildren(
        NugetPackageManagerTreeViewEntry nugetPackageManagerTreeViewEntry)
    {
        var children = new List<NugetPackageManagerTreeViewEntry>();

        switch (nugetPackageManagerTreeViewEntry.NugetPackageManagerTreeViewEntryKind)
        {
            case NugetPackageManagerTreeViewEntryKind.MarkupStringValue:
                break;
            case NugetPackageManagerTreeViewEntryKind.WrappedMarkupStringValue:
                var wrappedMarkupStringValue = (WrappedMarkupStringValue)nugetPackageManagerTreeViewEntry.Item;

                children.Add(new NugetPackageManagerTreeViewEntry(
                    wrappedMarkupStringValue.MarkupStringValue,
                    true,
                    wrappedMarkupStringValue.MarkupStringValue,
                    NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

                break;
            case NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable:
                var stringValueEnumerable = (IEnumerable<string>)nugetPackageManagerTreeViewEntry.Item;

                children.AddRange(stringValueEnumerable
                    .Select(x => new NugetPackageManagerTreeViewEntry(
                        x,
                        true,
                        (MarkupString)x,
                        NugetPackageManagerTreeViewEntryKind.MarkupStringValue)));

                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersion:
                var nugetPackageVersion = (NugetPackageVersionRecord)nugetPackageManagerTreeViewEntry.Item;

                children.Add(new NugetPackageManagerTreeViewEntry(
                    nugetPackageVersion.Downloads,
                    false,
                    (MarkupString)$"Downloads: <em>{nugetPackageVersion.Downloads:N0}</em>",
                    NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

                children.Add(new NugetPackageManagerTreeViewEntry(
                    nugetPackageVersion.Version,
                    false,
                    (MarkupString)$"Version: <em>{nugetPackageVersion.Version}</em>",
                    NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

                children.Add(new NugetPackageManagerTreeViewEntry(
                    nugetPackageVersion.AtId,
                    false,
                    (MarkupString)$"@id: <em>{nugetPackageVersion.AtId}</em>",
                    NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackageVersionEnumerable:
                var nugetPackageVersionEnumerable =
                    (IEnumerable<NugetPackageVersionRecord>)nugetPackageManagerTreeViewEntry.Item;

                children.AddRange(nugetPackageVersionEnumerable
                    .Select(x => new NugetPackageManagerTreeViewEntry(
                        x,
                        true,
                        (MarkupString)x.Version,
                        NugetPackageManagerTreeViewEntryKind.NugetPackageVersion)));

                break;
            case NugetPackageManagerTreeViewEntryKind.NugetPackage:
                var nugetPackage = (NugetPackageRecord)nugetPackageManagerTreeViewEntry.Item;

                GetNugetPackageRecordChildren(ref children, nugetPackage);

                break;
        }

        return Task.FromResult(children.AsEnumerable());
    }

    private void ThemeTreeViewOnEnterKeyDown(
        TreeViewKeyboardEventDto<NugetPackageManagerTreeViewEntry> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void ThemeTreeViewOnSpaceKeyDown(
        TreeViewKeyboardEventDto<NugetPackageManagerTreeViewEntry> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void ThemeTreeViewOnDoubleClick(
        TreeViewMouseEventDto<NugetPackageManagerTreeViewEntry> treeViewMouseEventDto)
    {
        treeViewMouseEventDto.ToggleIsExpanded.Invoke();
    }

    private IEnumerable<NugetPackageManagerTreeViewEntry> GetNugetPackageRecordChildren(
        ref List<NugetPackageManagerTreeViewEntry> children,
        NugetPackageRecord nugetPackageRecord)
    {
        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Title,
            false,
            (MarkupString)$"Title: <em>{nugetPackageRecord.Title}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.TotalDownloads,
            false,
            (MarkupString)$"Total Downloads: <em>{nugetPackageRecord.TotalDownloads:N0}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Version,
            false,
            (MarkupString)$"Recent Version: <em>{nugetPackageRecord.Version}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Versions.OrderByDescending(x => x.Version),
            true,
            (MarkupString)"Versions",
            NugetPackageManagerTreeViewEntryKind.NugetPackageVersionEnumerable));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Authors,
            true,
            (MarkupString)"Authors",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable));

        children.Add(new NugetPackageManagerTreeViewEntry(
            new WrappedMarkupStringValue { MarkupStringValue = (MarkupString)nugetPackageRecord.Description },
            true,
            (MarkupString)"Description",
            NugetPackageManagerTreeViewEntryKind.WrappedMarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Id,
            false,
            (MarkupString)$"Id: <em>{nugetPackageRecord.Id}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Owners,
            true,
            (MarkupString)"Owners",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Registration,
            false,
            (MarkupString)$"Registration: <em>{nugetPackageRecord.Registration}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            new WrappedMarkupStringValue { MarkupStringValue = (MarkupString)nugetPackageRecord.Summary },
            true,
            (MarkupString)"Summary",
            NugetPackageManagerTreeViewEntryKind.WrappedMarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Tags,
            true,
            (MarkupString)"Tags",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValueEnumerable));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Type,
            false,
            (MarkupString)$"Type: <em>{nugetPackageRecord.Type}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.Verified,
            false,
            (MarkupString)$"Verified: <em>{nugetPackageRecord.Verified}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.AtId,
            false,
            (MarkupString)$"@id: <em>{nugetPackageRecord.AtId}</em>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.IconUrl,
            false,
            (MarkupString)$"IconUrl: <a href='{nugetPackageRecord.IconUrl}'>{nugetPackageRecord.IconUrl}</a>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.LicenseUrl,
            false,
            (MarkupString) $"LicenseUrl: <a href='{nugetPackageRecord.LicenseUrl}'>{nugetPackageRecord.LicenseUrl}</a>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

        children.Add(new NugetPackageManagerTreeViewEntry(
            nugetPackageRecord.ProjectUrl,
            false,
            (MarkupString)$"ProjectUrl: <a href='{nugetPackageRecord.ProjectUrl}'>{nugetPackageRecord.ProjectUrl}</a>",
            NugetPackageManagerTreeViewEntryKind.MarkupStringValue));

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
        public NugetPackageManagerTreeViewEntry(
            object item,
            bool isExpandable,
            MarkupString markupStringTitleDisplay,
            NugetPackageManagerTreeViewEntryKind nugetPackageManagerTreeViewEntryKind)
        {
            Item = item;
            IsExpandable = isExpandable;
            MarkupStringTitleDisplay = markupStringTitleDisplay;
            NugetPackageManagerTreeViewEntryKind = nugetPackageManagerTreeViewEntryKind;
        }
        
        public object Item { get; } = null!;
        public bool IsExpandable { get; }
        public MarkupString MarkupStringTitleDisplay { get; }
        public NugetPackageManagerTreeViewEntryKind NugetPackageManagerTreeViewEntryKind { get; }
    }

    /// <summary>
    ///     This class is used when a string's value is very large and should have its own
    ///     child to hide the text until desired that it is shown.
    /// </summary>
    public class WrappedMarkupStringValue
    {
        public MarkupString MarkupStringValue { get; set; }
    }
}