using BlazorStudio.ClassLib.Menu;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.Dropdown;
using BlazorCommon.RazorLib.Menu;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.DialogCase;
using BlazorCommon.RazorLib.Store.NotificationCase;
using BlazorCommon.RazorLib.TreeView;
using BlazorCommon.RazorLib.TreeView.Commands;
using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.InputFile;
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

namespace BlazorStudio.RazorLib.FolderExplorer.InternalComponents;

public partial class FolderExplorerContextMenu : ComponentBase
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
    public ITreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly DropdownKey ContextMenuEventDropdownKey = DropdownKey.NewDropdownKey();
    
    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    public static TreeViewNoType? ParentOfCutFile;
    
    private MenuRecord GetMenuRecord(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        if (treeViewCommandParameter.TargetNode is null)
            return MenuRecord.Empty;
        
        var menuRecords = new List<MenuOptionRecord>();
        
        var treeViewModel = treeViewCommandParameter.TargetNode;
        var parentTreeViewModel = treeViewModel.Parent;

        var parentTreeViewAbsoluteFilePath = parentTreeViewModel as TreeViewAbsoluteFilePath;
        
        if (treeViewModel is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath ||
            treeViewAbsoluteFilePath.Item is null)
        {
            return MenuRecord.Empty;
        }
        
        if (treeViewAbsoluteFilePath.Item.IsDirectory)
        {
            menuRecords.AddRange(
                GetFileMenuOptions(treeViewAbsoluteFilePath, parentTreeViewAbsoluteFilePath)
                    .Union(GetDirectoryMenuOptions(treeViewAbsoluteFilePath))
                    .Union(GetDebugMenuOptions(treeViewAbsoluteFilePath)));
        }
        else
        {
            menuRecords.AddRange(
                GetFileMenuOptions(treeViewAbsoluteFilePath, parentTreeViewAbsoluteFilePath)
                    .Union(GetDebugMenuOptions(treeViewAbsoluteFilePath)));
        }
        
        return new MenuRecord(
            menuRecords.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDirectoryMenuOptions(TreeViewAbsoluteFilePath treeViewModel)
    {
        return new[]
        {
            CommonMenuOptionsFactory.NewEmptyFile(
                treeViewModel.Item,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.NewDirectory(
                treeViewModel.Item,
                async () => await ReloadTreeViewModel(treeViewModel)),
            CommonMenuOptionsFactory.PasteClipboard(
                treeViewModel.Item,
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
        TreeViewAbsoluteFilePath treeViewModel,
        TreeViewAbsoluteFilePath? parentTreeViewModel)
    {
        return new[]
        {
            CommonMenuOptionsFactory.CopyFile(
                treeViewModel.Item,
                () => NotifyCopyCompleted(treeViewModel.Item)),
            CommonMenuOptionsFactory.CutFile(
                treeViewModel.Item,
                () => NotifyCutCompleted(treeViewModel.Item, parentTreeViewModel)),
            CommonMenuOptionsFactory.DeleteFile(
                treeViewModel.Item,
                async () =>
                {
                    await ReloadTreeViewModel(parentTreeViewModel);
                }),
            CommonMenuOptionsFactory.RenameFile(
                treeViewModel.Item,
                Dispatcher,
                async ()  =>
                {
                    await ReloadTreeViewModel(parentTreeViewModel);
                }),
        };
    }
    
    private MenuOptionRecord[] GetDebugMenuOptions(
        TreeViewAbsoluteFilePath treeViewModel)
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
            FolderExplorerDisplay.TreeViewFolderExplorerContentStateKey, 
            treeViewModel);
        
        TreeViewService.MoveUp(
            FolderExplorerDisplay.TreeViewFolderExplorerContentStateKey,
            false);
    }
    
    private Task NotifyCopyCompleted(IAbsoluteFilePath absoluteFilePath)
    {
        var notificationInformative  = new NotificationRecord(
            NotificationKey.NewNotificationKey(), 
            "Copy Action",
            CommonComponentRenderers.InformativeNotificationRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(IInformativeNotificationRendererType.Message), 
                    $"Copied: {absoluteFilePath.FilenameWithExtension}"
                },
            },
            TimeSpan.FromSeconds(3));

        Dispatcher.Dispatch(
            new NotificationRecordsCollection.RegisterAction(
                notificationInformative));

        return Task.CompletedTask;
    }
    
    private Task NotifyCutCompleted(
        IAbsoluteFilePath absoluteFilePath,
        TreeViewAbsoluteFilePath? parentTreeViewModel)
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
                    $"Cut: {absoluteFilePath.FilenameWithExtension}"
                },
            },
            TimeSpan.FromSeconds(3));
        
        Dispatcher.Dispatch(
            new NotificationRecordsCollection.RegisterAction(
                notificationInformative));

        return Task.CompletedTask;
    }
    
    public static string GetContextMenuCssStyleString(ITreeViewCommandParameter? treeViewCommandParameter)
    {
        if (treeViewCommandParameter?.ContextMenuFixedPosition is null)
            return "display: none;";
        
        var left = 
            $"left: {treeViewCommandParameter.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";
        
        var top = 
            $"top: {treeViewCommandParameter.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}
