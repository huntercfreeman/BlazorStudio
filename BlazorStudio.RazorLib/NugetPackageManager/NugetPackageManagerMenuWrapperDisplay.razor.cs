using System.Collections.Immutable;
using System.Diagnostics;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.NugetPackageManager;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.NugetPackageManager;

public partial class NugetPackageManagerMenuWrapperDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public TreeViewContextMenuEventDto<NugetPackageManagerDisplay.NugetPackageManagerTreeViewEntry> ContextMenuEventDto
    {
        get;
        set;
    } = null!;
    [Parameter]
    public Project SelectProjectToModify { get; set; } = null!;

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewContextMenuEventDto<NugetPackageManagerDisplay.NugetPackageManagerTreeViewEntry> contextMenuEventDto)
    {
        var menuOptionRecords = new List<MenuOptionRecord>();

        if (contextMenuEventDto.Item.NugetPackageManagerTreeViewEntryKind ==
            NugetPackageManagerDisplay.NugetPackageManagerTreeViewEntryKind.NugetPackage)
        {
            var nugetPackageRecord = (NugetPackageRecord)contextMenuEventDto.Item.Item;

            var versionStrings = nugetPackageRecord.Versions
                .OrderByDescending(x => x.Version);

            var childrenMenu = versionStrings
                .Select(versionRecord => new MenuOptionRecord(
                    MenuOptionKey.NewMenuOptionKey(),
                    versionRecord.Version,
                    ImmutableList<MenuOptionRecord>.Empty,
                    () => AddNugetReference(nugetPackageRecord, versionRecord),
                    MenuOptionKind.Read))
                .ToImmutableList();

            var referenceNugetPackageForm = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                "Add Reference",
                childrenMenu,
                null,
                MenuOptionKind.Read);

            menuOptionRecords.Add(referenceNugetPackageForm);
        }

        return menuOptionRecords.Any()
            ? menuOptionRecords
            : new[]
            {
                new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                    "No Context Menu Options for this item",
                    ImmutableList<MenuOptionRecord>.Empty,
                    null,
                    MenuOptionKind.Read),
            };
    }

    private string FormatDotNetAddNugetPackageReferenceToProject(NugetPackageRecord nugetPackageRecord,
        NugetPackageVersionRecord versionRecord)
    {
        return
            $"dotnet add \"{SelectProjectToModify.FilePath}\" package \"{nugetPackageRecord.Id}\" --version {versionRecord.Version}";
    }

    private void AddNugetReference(NugetPackageRecord nugetPackageRecord,
        NugetPackageVersionRecord versionRecord)
    {
        var output = string.Empty;

        void OnStart()
        {
        }

        void OnEnd(Process finishedProcess)
        {
        }

        Dispatcher
            .Dispatch(new EnqueueProcessOnTerminalEntryAction(
                TerminalStateFacts.GeneralTerminalEntry.TerminalEntryKey,
                FormatDotNetAddNugetPackageReferenceToProject(nugetPackageRecord, versionRecord),
                null,
                OnStart,
                OnEnd,
                null,
                null,
                data => output = data,
                CancellationToken.None));
    }
}