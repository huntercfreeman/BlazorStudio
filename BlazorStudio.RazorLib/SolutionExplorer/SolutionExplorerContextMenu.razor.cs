using BlazorStudio.ClassLib.Menu;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorALaCarte.DialogNotification.Notification;
using BlazorALaCarte.DialogNotification.Store.DialogCase;
using BlazorALaCarte.DialogNotification.Store.NotificationCase;
using BlazorALaCarte.Shared.Dropdown;
using BlazorALaCarte.Shared.Menu;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorALaCarte.TreeView.Events;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.ProgramExecutionCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorStudio.RazorLib.CSharpProjectForm;
using BlazorStudio.RazorLib.DotNetSolutionForm;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<WorkspaceState> WorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewContextMenuEvent TreeViewContextMenuEvent { get; set; } = null!;

    public static readonly DropdownKey ContextMenuEventDropdownKey = DropdownKey.NewDropdownKey();
    
    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    public static TreeViewNoType? ParentOfCutFile;
    
    private MenuRecord GetMenuRecord(
        TreeViewContextMenuEvent treeViewContextMenuEvent)
    {
        var menuRecords = new List<MenuOptionRecord>();
        
        var treeViewModel = treeViewContextMenuEvent.TreeViewNoType;
        var parentTreeViewModel = treeViewModel.Parent;

        var parentTreeViewNamespacePath = parentTreeViewModel as TreeViewNamespacePath;
        
        if (treeViewModel is not TreeViewNamespacePath treeViewNamespacePath ||
            treeViewNamespacePath.Item is null)
        {
            return new MenuRecord(new []
            {
                new MenuOptionRecord(
                    "No context menu options for this item.",
                    MenuOptionKind.Other,
                    () => { })
            }.ToImmutableArray());
        }
        
        if (treeViewNamespacePath.Item.AbsoluteFilePath.IsDirectory)
        {
            menuRecords.AddRange(
                GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                    .Union(GetDirectoryMenuOptions(treeViewNamespacePath))
                    .Union(GetDebugMenuOptions(treeViewNamespacePath)));
        }
        else
        {
            switch (treeViewNamespacePath.Item.AbsoluteFilePath.ExtensionNoPeriod)
            {
                case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                    if (treeViewNamespacePath.Parent is null ||
                        treeViewNamespacePath.Parent is TreeViewAdhoc)
                    {
                        menuRecords.AddRange(
                            GetDotNetSolutionMenuOptions(treeViewNamespacePath)
                                .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                    menuRecords.AddRange(
                        GetCSharpProjectMenuOptions(treeViewNamespacePath)
                            .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                    break;
                default:
                    menuRecords.AddRange(
                        GetFileMenuOptions(treeViewNamespacePath, parentTreeViewNamespacePath)
                            .Union(GetDebugMenuOptions(treeViewNamespacePath)));
                    break;
            }
        }
        
        return new MenuRecord(
            menuRecords.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDotNetSolutionMenuOptions(TreeViewNamespacePath treeViewModel)
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
    
    private MenuOptionRecord[] GetCSharpProjectMenuOptions(TreeViewNamespacePath treeViewModel)
    {
        var parentDirectory = (IAbsoluteFilePath)treeViewModel.Item.AbsoluteFilePath.Directories.Last();

        // likely a .NET Solution
        var treeViewParent = treeViewModel.Parent as TreeViewNamespacePath;

        var solutionExplorerState = SolutionExplorerStateWrap.Value;

        var project = solutionExplorerState.Solution?.Projects
            .SingleOrDefault(x => x.Name == treeViewModel.Item.AbsoluteFilePath.FileNameNoExtension);

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
                        ParentOfCutFile;
                    
                    ParentOfCutFile = null;
                    
                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);
                    
                    await ReloadTreeViewModel(treeViewModel);
                }),
            CommonMenuOptionsFactory.AddProjectToProjectReference(
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher, () => { return Task.CompletedTask; }),
            new MenuOptionRecord(
                "Set as Startup Project",
                MenuOptionKind.Other,
                () => Dispatcher.Dispatch(
                    new ProgramExecutionState.SetStartupProjectAbsoluteFilePathAction(
                        treeViewModel.Item.AbsoluteFilePath))),
            CommonMenuOptionsFactory.RemoveCSharpProjectReferenceFromSolution(
                treeViewParent,
                treeViewModel,
                TerminalSessionsStateWrap.Value
                    .TerminalSessionMap[
                        TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY],
                Dispatcher, () =>
                {
                    if (project is not null)
                    {
                        var solution = SolutionExplorerStateWrap.Value.Solution;
                        
                        if (solution is not null)
                        {
                            var requestSetSolutionExplorerStateAction = 
                                new SolutionExplorerState.RequestSetSolutionAction(
                                    solution.RemoveProject(project.Id));
                            
                            Dispatcher.Dispatch(requestSetSolutionExplorerStateAction);
                        }
                    }

                    return Task.CompletedTask;
                }),
        };
    }
    
    private MenuOptionRecord[] GetDirectoryMenuOptions(TreeViewNamespacePath treeViewModel)
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
                        ParentOfCutFile;
                    
                    ParentOfCutFile = null;
                    
                    if (localParentOfCutFile is not null)
                        await ReloadTreeViewModel(localParentOfCutFile);
                    
                    await ReloadTreeViewModel(treeViewModel);
                }),
        };
    }
    
    private MenuOptionRecord[] GetFileMenuOptions(
        TreeViewNamespacePath treeViewModel,
        TreeViewNamespacePath? parentTreeViewModel)
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
                async () =>
                {
                    await ReloadTreeViewModel(parentTreeViewModel);
                }),
            CommonMenuOptionsFactory.RenameFile(
                treeViewModel.Item.AbsoluteFilePath,
                Dispatcher,
                async ()  =>
                {
                    await ReloadTreeViewModel(parentTreeViewModel);
                }),
        };
    }
    
    private MenuOptionRecord[] GetDebugMenuOptions(
        TreeViewNamespacePath treeViewModel)
    {
        return new MenuOptionRecord[]
        {
            // new MenuOptionRecord(
            //     $"namespace: {treeViewModel.Item.Namespace}",
            //     MenuOptionKind.Read)
        };
    }

    private void OpenNewCSharpProjectDialog(NamespacePath solutionNamespacePath)
    {
        var dialogRecord = new DialogRecord(
            DialogKey.NewDialogKey(), 
            "New C# Project",
            typeof(CSharpProjectFormDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CSharpProjectFormDisplay.SolutionNamespacePath),
                    solutionNamespacePath
                }
            })
        {
            IsResizable = true
        };
        
        Dispatcher.Dispatch(
            new DialogRecordsCollection.RegisterAction(
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
                        .FormatAddExistingProjectToSolution(
                            solutionNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString(),
                            afp.GetAbsoluteFilePathString());
                    
                    var addExistingProjectToSolutionTerminalCommand = new TerminalCommand(
                        TerminalCommandKey.NewTerminalCommandKey(), 
                        localInterpolatedAddExistingProjectToSolutionCommand,
                        null,
                        CancellationToken.None,
                        async () =>
                        {
                            // Add the C# project to the workspace
                            //
                            // Cannot find another way as of 2022-11-09
                            // to add the C# project to the workspace
                            // other than reloading the solution.
                            {
                                var mSBuildWorkspace = ((MSBuildWorkspace)WorkspaceStateWrap.Value.Workspace);

                                var solution = SolutionExplorerStateWrap.Value.Solution;

                                if (mSBuildWorkspace is not null &&
                                    solution is not null &&
                                    solution.FilePath is not null)
                                {
                                    mSBuildWorkspace.CloseSolution();
                            
                                    solution = await mSBuildWorkspace
                                        .OpenSolutionAsync(solution.FilePath);
                            
                                    var requestSetSolutionExplorerStateAction = 
                                        new SolutionExplorerState.RequestSetSolutionAction(
                                            solution);
                            
                                    Dispatcher.Dispatch(requestSetSolutionExplorerStateAction);
                                }
                            }
                        });
                    
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

    /// <summary>
    /// This method I believe is causing bugs
    /// <br/><br/>
    /// For example, when removing a C# Project the
    /// solution is reloaded and a new root is made.
    /// <br/><br/>
    /// Then there is a timing issue where the new root is made and set
    /// as the root. But this method erroneously reloads the old root.
    /// </summary>
    /// <param name="treeViewModel"></param>
    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();
        
        TreeViewService.ReRenderNode(
            SolutionExplorerDisplay.TreeViewSolutionExplorerStateKey, 
            treeViewModel);
        
        TreeViewService.MoveUp(
            SolutionExplorerDisplay.TreeViewSolutionExplorerStateKey,
            false);
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
            },
            TimeSpan.FromSeconds(3));

        Dispatcher.Dispatch(
            new NotificationRecordsCollection.RegisterAction(
                notificationInformative));

        return Task.CompletedTask;
    }
    
    private Task NotifyCutCompleted(
        NamespacePath namespacePath,
        TreeViewNamespacePath? parentTreeViewModel)
    {
        ParentOfCutFile = parentTreeViewModel;
        
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
            },
            TimeSpan.FromSeconds(3));
        
        Dispatcher.Dispatch(
            new NotificationRecordsCollection.RegisterAction(
                notificationInformative));

        return Task.CompletedTask;
    }
    
    public static string GetContextMenuCssStyleString(TreeViewContextMenuEvent? treeViewContextMenuEvent)
    {
        if (treeViewContextMenuEvent is null)
            return "display: none;";
        
        var left = 
            $"left: {treeViewContextMenuEvent.ContextMenuFixedPosition.LeftPositionInPixels.ToString(System.Globalization.CultureInfo.InvariantCulture)}px;";
        
        var top = 
            $"top: {treeViewContextMenuEvent.ContextMenuFixedPosition.TopPositionInPixels.ToString(System.Globalization.CultureInfo.InvariantCulture)}px;";

        return $"{left} {top}";
    }
}
