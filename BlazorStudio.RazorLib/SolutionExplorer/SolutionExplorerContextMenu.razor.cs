using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.TreeView;
using BlazorStudio.RazorLib.TreeView;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerContextMenu : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewDisplayContextMenuEvent<IAbsoluteFilePath> TreeViewDisplayContextMenuEvent { get; set; } = null!;
    
    private MenuRecord GetContextMenu(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var menuRecords = new List<MenuOptionRecord>();
        
        switch (treeViewModel.Item.ExtensionNoPeriod)
        {
            default:
                menuRecords.AddRange(
                    GetFileMenuOptions());
                break;
        }
        
        return new MenuRecord(
            menuRecords.ToImmutableArray());
    }

    private string GetStyleForContextMenu(MouseEventArgs? mouseEventArgs)
    {
        if (mouseEventArgs is null)
            return string.Empty;

        return 
            $"position: fixed; left: {mouseEventArgs.ClientX}px; top: {mouseEventArgs.ClientY}px;";
    }

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions()
    {
        return Array.Empty<MenuOptionRecord>();
    }
    
    private MenuOptionRecord[] GetCSharpProjectMenuOptions()
    {
        return Array.Empty<MenuOptionRecord>();
    }
    
    private MenuOptionRecord[] GetFileMenuOptions()
    {
        return new[]
        {
            new MenuOptionRecord(
                "Copy"),
            new MenuOptionRecord(
                "Cut"),
            new MenuOptionRecord(
                "Delete"),
            new MenuOptionRecord(
                "Rename"),
        };
    }
}