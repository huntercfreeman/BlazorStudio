using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.ProgramExecutionCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
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
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewDisplayContextMenuEvent<IAbsoluteFilePath> TreeViewDisplayContextMenuEvent { get; set; } = null!;
    
    private MenuRecord GetContextMenu(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var menuRecords = new List<MenuOptionRecord>();

        if (treeViewModel.Item.IsDirectory)
        {
            menuRecords.AddRange(
                GetFileMenuOptions()
                    .Union(GetDirectoryMenuOptions(treeViewModel)));
        }
        else
        {
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
            MenuOptionKind.Other,
            () => OpenNewCSharpProjectDialog(treeViewModel.Item));
        
        var addExistingCSharpProject = new MenuOptionRecord(
            "Existing C# Project",
            MenuOptionKind.Other,
            () => AddExistingProjectToSolution(treeViewModel.Item));
        
        return new[]
        {
            new MenuOptionRecord(
                "Add",
                MenuOptionKind.Other,
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
                MenuOptionKind.Other,
                () => Dispatcher.Dispatch(
                    new ProgramExecutionState.SetStartupProjectAbsoluteFilePathAction(
                        treeViewModel.Item))),
        };
    }
    
    private MenuOptionRecord[] GetDirectoryMenuOptions(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        return new[]
        {
            CommonMenuOptionsFactory.NewEmptyFile(treeViewModel.Item)
        };
    }
    
    private MenuOptionRecord[] GetFileMenuOptions()
    {
        return new[]
        {
            new MenuOptionRecord(
                "Copy",
                MenuOptionKind.Other),
            new MenuOptionRecord(
                "Cut",
                MenuOptionKind.Other),
            new MenuOptionRecord(
                "Delete",
                MenuOptionKind.Other),
            new MenuOptionRecord(
                "Rename",
                MenuOptionKind.Other),
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
    
    private void AddExistingProjectToSolution(IAbsoluteFilePath solutionAbsoluteFilePath)
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "Existing C# Project to add to solution",
                async afp =>
                {
                    if (afp is null)
                        return;
                    
                    var localInterpolatedAddExistingProjectToSolutionCommand = DotNetCliFacts
                        .AddExistingProjectToSolution(
                            solutionAbsoluteFilePath.GetAbsoluteFilePathString(),
                            afp.GetAbsoluteFilePathString());
                    
                    var addExistingProjectToSolutionTerminalCommand = new TerminalCommand(
                        TerminalCommandKey.NewTerminalCommandKey(), 
                        localInterpolatedAddExistingProjectToSolutionCommand,
                        null,
                        CancellationToken.None);
                    
                    var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

                    await generalTerminalSession
                        .EnqueueCommandAsync(addExistingProjectToSolutionTerminalCommand);
                },
                afp =>
                {
                    if (afp is null ||
                        afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(
                        afp.ExtensionNoPeriod
                            .EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
                },
                new[]
                {
                    new InputFilePattern(
                        "C# Project",
                        afp => 
                            afp.ExtensionNoPeriod
                                .EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
                }.ToImmutableArray()));
    }
}