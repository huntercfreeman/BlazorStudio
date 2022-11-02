using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.ProgramExecutionCase;
using BlazorStudio.ClassLib.TreeView;
using BlazorStudio.RazorLib.CSharpProjectForm;
using BlazorStudio.RazorLib.DotNetSolutionForm;
using BlazorStudio.RazorLib.TreeView;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerContextMenu : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewDisplayContextMenuEvent<IAbsoluteFilePath> TreeViewDisplayContextMenuEvent { get; set; } = null!;
    
    private MenuRecord GetContextMenu(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var menuRecords = new List<MenuOptionRecord>();
        
        switch (treeViewModel.Item.ExtensionNoPeriod)
        {
            case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                menuRecords.AddRange(
                    GetFileMenuOptions()
                        .Union(GetDotNetSolutionMenuOptions(treeViewModel)));
                break;
            case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                menuRecords.AddRange(
                    GetFileMenuOptions()
                        .Union(GetCSharpProjectMenuOptions(treeViewModel)));
                break;
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

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        // TODO: Add menu options for non C# projects perhaps a more generic option is good

        var addNewCSharpProject = new MenuOptionRecord(
            "New C# Project",
            () => OpenNewCSharpProjectDialog(treeViewModel.Item));
        
        var addExistingCSharpProject = new MenuOptionRecord(
            "Existing C# Project",
            () => Dispatcher.Dispatch(
                new ProgramExecutionState.SetStartupProjectAbsoluteFilePathAction(
                    treeViewModel.Item)));
        
        return new[]
        {
            new MenuOptionRecord(
                "Add",
                SubMenu: new MenuRecord(
                    new MenuOptionRecord[]
                    {
                        addNewCSharpProject,
                        addExistingCSharpProject
                    }.ToImmutableArray())),
        };
    }
    
    private MenuOptionRecord[] GetCSharpProjectMenuOptions(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        return new[]
        {
            new MenuOptionRecord(
                "Set as Startup Project",
                () => Dispatcher.Dispatch(
                    new ProgramExecutionState.SetStartupProjectAbsoluteFilePathAction(
                        treeViewModel.Item))),
        };
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
    
    private void OpenNewCSharpProjectDialog(IAbsoluteFilePath solutionAbsoluteFilePath)
    {
        var dialogRecord = new DialogRecord(
            DialogKey.NewDialogKey(), 
            "New .NET Solution",
            typeof(CSharpProjectFormDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CSharpProjectFormDisplay.SolutionAbsoluteFilePath),
                    solutionAbsoluteFilePath
                }
            });
        
        Dispatcher.Dispatch(
            new RegisterDialogRecordAction(
                dialogRecord));
    }
}