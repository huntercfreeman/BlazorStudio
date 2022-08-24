using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.NugetPackageManager;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.RazorLib.TreeViewCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NugetPackageManager;

public partial class NugetPackageManagerMenuWrapperDisplay : ComponentBase
{
    [Parameter]
    public TreeViewContextMenuEventDto<NugetPackageManagerDisplay.NugetPackageManagerTreeViewEntry> ContextMenuEventDto { get; set; } = null!;

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewContextMenuEventDto<NugetPackageManagerDisplay.NugetPackageManagerTreeViewEntry> contextMenuEventDto)
    {
        var menuOptionRecords = new List<MenuOptionRecord>();

        if (contextMenuEventDto.Item.NugetPackageManagerTreeViewEntryKind == 
            NugetPackageManagerDisplay.NugetPackageManagerTreeViewEntryKind.NugetPackage)
        {
            var nugetPackageRecord = (NugetPackageRecord)contextMenuEventDto.Item.Item;

            var versionStrings = nugetPackageRecord.Versions
                .OrderByDescending(x => x.Version)
                .Select(x => x.Version);

            var childrenMenu = versionStrings
                .Select(version => new MenuOptionRecord(
                    MenuOptionKey.NewMenuOptionKey(),
                    version,
                    ImmutableList<MenuOptionRecord>.Empty,
                    null,
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
                    MenuOptionKind.Read)
            };
    }
}