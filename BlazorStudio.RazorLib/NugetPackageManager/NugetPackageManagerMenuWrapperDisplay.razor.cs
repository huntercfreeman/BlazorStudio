using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
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
        
        // var createNewEmptyFile = MenuOptionFacts.File
        //     .ConstructCreateNewEmptyFile(typeof(CreateNewFileForm),
        //         new Dictionary<string, object?>()
        //         {
        //             {
        //                 nameof(CreateNewFileForm.ParentDirectory),
        //                 contextMenuEventDto.Item
        //             },
        //             {
        //                 nameof(CreateNewFileForm.OnAfterSubmitForm),
        //                 new Action<string, string, bool>((parentDirectoryAbsoluteFilePathString, filename, _) => 
        //                     CreateNewEmptyFileFormOnAfterSubmitForm(parentDirectoryAbsoluteFilePathString,
        //                         filename, 
        //                         ContextMenuEventDto.Item))
        //             },
        //             {
        //                 nameof(CreateNewFileForm.OnAfterCancelForm),
        //                 new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
        //             },
        //         });
        
        
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