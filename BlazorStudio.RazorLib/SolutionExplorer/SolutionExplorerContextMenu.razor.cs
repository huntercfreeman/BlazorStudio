using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.NotificationCase;
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
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewDisplayContextMenuEvent<NamespacePath> TreeViewDisplayContextMenuEvent { get; set; } = null!;

    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    private static TreeViewModel<NamespacePath>? _parentOfCutFile;

    private MenuRecord GetContextMenu(
        TreeViewDisplayContextMenuEvent<NamespacePath> treeViewDisplayContextMenuEvent)
    {
        var menuRecords = new List<MenuOptionRecord>();
        
        // TODO: I don't like what I'm doing here with treeViewDisplayContextMenuEvent it seems verbose perhaps revisit this
        var treeViewModel = treeViewDisplayContextMenuEvent.TreeViewDisplay.TreeViewModel;
        var parentTreeViewModel = treeViewDisplayContextMenuEvent
            .TreeViewDisplay.InternalParameters.ParentTreeViewDisplay?.TreeViewModel;
        
        if (treeViewModel.Item.AbsoluteFilePath.IsDirectory)
        {
            menuRecords.AddRange(
                GetFileMenuOptions(treeViewModel, parentTreeViewModel)
                    .Union(GetDirectoryMenuOptions(treeViewModel))
                    .Union(GetDebugMenuOptions(treeViewModel)));
        }
        else
        {
            switch (treeViewModel.Item.AbsoluteFilePath.ExtensionNoPeriod)
            {
                case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                    menuRecords.AddRange(
                        GetDotNetSolutionMenuOptions(treeViewModel)
                            .Union(GetDebugMenuOptions(treeViewModel)));
                    break;
                case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                    menuRecords.AddRange(
                        GetCSharpProjectMenuOptions(treeViewModel)
                            .Union(GetDebugMenuOptions(treeViewModel)));
                    break;
                default:
                    menuRecords.AddRange(
                        GetFileMenuOptions(treeViewModel, parentTreeViewModel)
                            .Union(GetDebugMenuOptions(treeViewModel)));
                    break;
            }
        }
        
        return new MenuRecord(
            menuRecords.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions(TreeViewModel<NamespacePath> treeViewModel)
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
    
    private MenuOptionRecord[] GetCSharpProjectMenuOptions(TreeViewModel<NamespacePath> treeViewModel)
    {
        var parentDirectory = (IAbsoluteFilePath)treeViewModel.Item.AbsoluteFilePath.Directories.Last();
        
        return new[]
        {
            CommonMenuOptionsFactory.NewEmptyFile(
                parentDirectory,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewTemplatedFile(
                treeViewModel.Item,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewDirectory(
                parentDirectory,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.PasteClipboard(
                parentDirectory,
                async () =>
                {
                    var localParentOfCutFile = 
                        _parentOfCutFile;
                    
                    _parentOfCutFile = null;
                    
                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);
                    
                    await ReloadTreeViewModel(treeViewModel);
                }),
            new MenuOptionRecord(
                "Set as Startup Project",
                MenuOptionKind.Other,
                () => Dispatcher.Dispatch(
                    new ProgramExecutionState.SetStartupProjectAbsoluteFilePathAction(
                        treeViewModel.Item.AbsoluteFilePath))),
        };
    }
    
    private MenuOptionRecord[] GetDirectoryMenuOptions(TreeViewModel<NamespacePath> treeViewModel)
    {
        return new[]
        {
            CommonMenuOptionsFactory.NewEmptyFile(
                treeViewModel.Item.AbsoluteFilePath,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewTemplatedFile(
                treeViewModel.Item,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewDirectory(
                treeViewModel.Item.AbsoluteFilePath,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.PasteClipboard(
                treeViewModel.Item.AbsoluteFilePath,
                async () =>
                {
                    var localParentOfCutFile = 
                        _parentOfCutFile;
                    
                    _parentOfCutFile = null;
                    
                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);
                    
                    await ReloadTreeViewModel(treeViewModel);
                }),
        };
    }
    
    private MenuOptionRecord[] GetFileMenuOptions(
        TreeViewModel<NamespacePath> treeViewModel,
        TreeViewModel<NamespacePath>? parentTreeViewModel)
    {
        return new[]
        {
            CommonMenuOptionsFactory.CopyFile(
                treeViewModel.Item.AbsoluteFilePath,
                () => NotifyCopyCompleted(treeViewModel.Item)),
            CommonMenuOptionsFactory.CutFile(
                treeViewModel.Item.AbsoluteFilePath,
                () => NotifyCutCompleted(treeViewModel.Item, parentTreeViewModel)),
            CommonMenuOptionsFactory.DeleteFile(
                treeViewModel.Item.AbsoluteFilePath,
                async () => await ReloadTreeViewModel(parentTreeViewModel)),
            CommonMenuOptionsFactory.RenameFile(
                treeViewModel.Item.AbsoluteFilePath,
                async () => await ReloadTreeViewModel(parentTreeViewModel)),
        };
    }
    
    private MenuOptionRecord[] GetDebugMenuOptions(
        TreeViewModel<NamespacePath> treeViewModel)
    {
        return new[]
        {
            new MenuOptionRecord(
                $"namespace: {treeViewModel.Item.Namespace}",
                MenuOptionKind.Read)
        };
    }

    private void OpenNewCSharpProjectDialog(NamespacePath solutionNamespacePath)
    {
        var dialogRecord = new DialogRecord(
            DialogKey.NewDialogKey(), 
            "New .NET Solution",
            typeof(CSharpProjectFormDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CSharpProjectFormDisplay.SolutionNamespacePath),
                    solutionNamespacePath
                }
            });
        
        Dispatcher.Dispatch(
            new RegisterDialogRecordAction(
                dialogRecord));
    }
    
    private void AddExistingProjectToSolution(NamespacePath solutionNamespacePath)
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
                            solutionNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString(),
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

    private async Task ReloadTreeViewModel(
        TreeViewModel<NamespacePath>? treeViewModel)
    {
        if (treeViewModel is null)
            return;
        
        await treeViewModel.LoadChildrenFuncAsync(treeViewModel);
    }
    
    private Task NotifyCopyCompleted(NamespacePath namespacePath)
    {
        var notificationInformative  = new NotificationRecord(
            NotificationKey.NewNotificationKey(), 
            "Copy Action",
            CommonComponentRenderers.InformativeNotificationRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(IInformativeNotificationRendererType.Message), 
                    $"Copied: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
                },
            });
        
        Dispatcher.Dispatch(
            new NotificationState.RegisterNotificationAction(
                notificationInformative));

        return Task.CompletedTask;
    }
    
    private Task NotifyCutCompleted(
        NamespacePath namespacePath,
        TreeViewModel<NamespacePath>? parentTreeViewModel)
    {
        _parentOfCutFile = parentTreeViewModel;
        
        var notificationInformative  = new NotificationRecord(
            NotificationKey.NewNotificationKey(), 
            "Cut Action",
            CommonComponentRenderers.InformativeNotificationRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(IInformativeNotificationRendererType.Message), 
                    $"Cut: {namespacePath.AbsoluteFilePath.FilenameWithExtension}"
                },
            });
        
        Dispatcher.Dispatch(
            new NotificationState.RegisterNotificationAction(
                notificationInformative));

        return Task.CompletedTask;
    }
}