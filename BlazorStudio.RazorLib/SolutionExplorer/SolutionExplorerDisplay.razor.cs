using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor.Blazor.Web.Components;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.DialogCase;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using BlazorStudio.RazorLib.NewCSharpProject;
using BlazorStudio.ClassLib.Store.TerminalCase;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.RoslynWorkspaceState;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.SyntaxRootRender;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerDisplay : FluxorComponent, IDisposable
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<RoslynWorkspaceState> RoslynWorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    private void LoadFile(InputFileChangeEventArgs e)
    {
    }

    private bool _isInitialized;
    private TreeViewWrapKey _solutionExplorerTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrap<AbsoluteFilePathDotNet> _treeViewWrap = null!;
    private List<AbsoluteFilePathDotNet> _rootAbsoluteFilePaths;
    private RichErrorModel? _solutionExplorerStateWrapStateChangedRichErrorModel;
    private TreeViewWrapDisplay<AbsoluteFilePathDotNet>? _treeViewWrapDisplay;
    private Func<Task> _mostRecentRefreshContextMenuTarget;

    private Dimensions _fileDropdownDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Absolute,
        LeftCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 5
            }
        },
        TopCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                Value = 2.5
            }
        },
    };

    private DialogRecord _newCSharpProjectDialog;

    private DropdownKey _fileDropdownKey = DropdownKey.NewDropdownKey();
    private bool _loadingSln;

    protected override void OnInitialized()
    {
        SolutionExplorerStateWrap.StateChanged += SolutionExplorerStateWrap_StateChanged;
        SolutionStateWrap.StateChanged += SolutionStateWrapOnStateChanged;
        
        ContextStateSelector
            .Select(x => x.ContextRecords[ContextFacts.SolutionExplorerContext.ContextKey]);

        _newCSharpProjectDialog = new DialogRecord(
            DialogKey.NewDialogKey(),
            "New C# Project",
            typeof(NewCSharpProjectDialog),
            new Dictionary<string, object?>()
            {
                {
                    nameof(NewCSharpProjectDialog.OnProjectCreatedCallback), 
                    new Action<AbsoluteFilePathDotNet>(OnProjectCreatedCallback)
                }
            }
        );

        base.OnInitialized();
    }
    
    // protected override void OnAfterRender(bool firstRender)
    // {
    //     if (firstRender)
    //     {
    //         var ubuntuOsDebugSolution = "/home/hunter/RiderProjects/TestBlazorStudio/TestBlazorStudio.sln";
    //         var windowsOsDebugSolution = "C:\\Users\\hunte\\source\\BlazorDocumentationInteractive\\BlazorDocs.sln";
    //         
    //         AbsoluteFilePathDotNet solutionAbsoluteFilePath;
    //             
    //         if (File.Exists(ubuntuOsDebugSolution))
    //         {
    //             solutionAbsoluteFilePath =
    //                 new AbsoluteFilePathDotNet(ubuntuOsDebugSolution,
    //                     false, 
    //                     null);
    //         }
    //         else
    //         {
    //             solutionAbsoluteFilePath =
    //                 new AbsoluteFilePathDotNet(windowsOsDebugSolution,
    //                     false, 
    //                     null);
    //         }    
    //             
    //             
    //
    //         Dispatcher.Dispatch(new SetSolutionExplorerAction(solutionAbsoluteFilePath, SequenceKey.NewSequenceKey()));
    //     }       
    //     
    //     base.OnAfterRender(firstRender);
    // }

    private async void SolutionExplorerStateWrap_StateChanged(object? sender, EventArgs e)
    {
        var solutionExplorerState = SolutionExplorerStateWrap.Value;

        if (solutionExplorerState.SolutionAbsoluteFilePath is not null)
        {
            _isInitialized = false;
            _solutionExplorerStateWrapStateChangedRichErrorModel = null;
            _rootAbsoluteFilePaths = null;

            await InvokeAsync(StateHasChanged);

            _treeViewWrap = new TreeViewWrap<AbsoluteFilePathDotNet>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
            {
                try
                {
                    _loadingSln = true;

                    await InvokeAsync(StateHasChanged);

                    // Attempt to set the version of MSBuild.
                    VisualStudioInstance[] visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

                    // TODO: Let user choose MSBuild Version
                    var visualStudioInstance = visualStudioInstances[0];

                    if (!MSBuildLocator.IsRegistered)
                    {
                        MSBuildLocator.RegisterInstance(visualStudioInstance);
                    }
                    
                    var workspace = MSBuildWorkspace.Create();

                    var noTrailingSlashMSBuildPath = visualStudioInstance.MSBuildPath;
                    
                    if (visualStudioInstance.MSBuildPath.EndsWith(Path.DirectorySeparatorChar) ||
                        visualStudioInstance.MSBuildPath.EndsWith(Path.AltDirectorySeparatorChar))
                    {
                        noTrailingSlashMSBuildPath = noTrailingSlashMSBuildPath.Substring(0, noTrailingSlashMSBuildPath.Length - 1);
                    }
                    
                    var msBuildAbsoluteFilePath = new AbsoluteFilePath(noTrailingSlashMSBuildPath, true);
                    
                    var solution = await workspace
                        .OpenSolutionAsync(solutionExplorerState.SolutionAbsoluteFilePath
                            .GetAbsoluteFilePathString());

                    Dispatcher.Dispatch(new SetRoslynWorkspaceStateAction(workspace, visualStudioInstance, msBuildAbsoluteFilePath));
                    
                    var projects = new List<AbsoluteFilePathDotNet>();
                    
                    Dictionary<ProjectId, IndexedProject> localProjectMap = new();
                    Dictionary<AbsoluteFilePathStringValue, IndexedDocument> localFileDocumentMap = new();
                    Dictionary<AbsoluteFilePathStringValue, IndexedAdditionalDocument> localFileAdditionalDocumentMap = new();

                    foreach (var project in solution.Projects)
                    {
                        var projectAbsoluteFilePath = new AbsoluteFilePathDotNet(project.FilePath ?? "{null file path}", false, project.Id);
                        
                        projects.Add(projectAbsoluteFilePath);
                        localProjectMap.TryAdd(project.Id, new IndexedProject(project, projectAbsoluteFilePath));
                        
                        foreach (Document document in project.Documents)
                        {
                            if (document.FilePath is not null)
                            {
                                var absoluteFilePath = new AbsoluteFilePathDotNet(document.FilePath, false, project.Id);
                
                                _ = localFileDocumentMap
                                    .TryAdd(new AbsoluteFilePathStringValue(absoluteFilePath),
                                        new IndexedDocument(document, absoluteFilePath));
                            }
                        }
                        
                        foreach (TextDocument textDocument in project.AdditionalDocuments)
                        {
                            if (textDocument.FilePath is not null)
                            {
                                var absoluteFilePath = new AbsoluteFilePathDotNet(textDocument.FilePath, false, project.Id);
                
                                _ = localFileAdditionalDocumentMap
                                    .TryAdd(new AbsoluteFilePathStringValue(absoluteFilePath),
                                        new IndexedAdditionalDocument(textDocument, absoluteFilePath));
                            }
                        }
                    }
        
                    Dispatcher.Dispatch(new SetSolutionStateAction(solution,
                        localProjectMap.ToImmutableDictionary(),
                        localFileDocumentMap.ToImmutableDictionary(),
                        localFileAdditionalDocumentMap.ToImmutableDictionary()));
                    
                    _rootAbsoluteFilePaths = projects;
                }
                finally
                {
                    _loadingSln = false;
                }

                _isInitialized = true;

                await InvokeAsync(StateHasChanged);

                if (_treeViewWrapDisplay is not null)
                {
                    _treeViewWrapDisplay.Reload();
                }
            },
                $"{nameof(SolutionExplorerStateWrap_StateChanged)}",
                false,
                TimeSpan.FromSeconds(10),
                exception =>
                {
                    _isInitialized = true;
                    _solutionExplorerStateWrapStateChangedRichErrorModel = new RichErrorModel(
                        $"{nameof(SolutionExplorerStateWrap_StateChanged)}: {exception.Message}",
                        $"TODO: Add a hint");

                    InvokeAsync(StateHasChanged);

                    return _solutionExplorerStateWrapStateChangedRichErrorModel;
                });
        }
    }
    
    private void SolutionStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        
    }

    private async Task<IEnumerable<AbsoluteFilePathDotNet>> LoadAbsoluteFilePathChildrenAsync(AbsoluteFilePathDotNet absoluteFilePathDotNet)
    {
        if (absoluteFilePathDotNet.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
        {
            var indexedProjects = SolutionStateWrap.Value.ProjectIdToProjectMap.Values;

            return indexedProjects.Select(x => x.AbsoluteFilePathDotNet);
        }
        
        if (absoluteFilePathDotNet.ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_PROJECT)
        {
            var containingDirectory = absoluteFilePathDotNet.Directories.Last();

            if (containingDirectory is AbsoluteFilePath containingDirectoryAbsoluteFilePath)
            {
                var hiddenFiles = HiddenFileFacts
                    .GetHiddenFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

                var projectChildDirectoryAbsolutePaths = Directory
                    .GetDirectories(containingDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
                    .Where(x => hiddenFiles.All(hidden => !x.EndsWith(hidden)))
                    .Select(x => new AbsoluteFilePathDotNet(x, true, absoluteFilePathDotNet.ProjectId))
                    .ToList();

                var uniqueDirectories =
                    UniqueFileFacts.GetUniqueFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

                var foundUniqueDirectories = new List<AbsoluteFilePathDotNet>();
                var foundDefaultDirectories = new List<AbsoluteFilePathDotNet>();

                foreach (var directory in projectChildDirectoryAbsolutePaths)
                {
                    if (uniqueDirectories.Any(unique => directory.FileNameNoExtension == unique))
                    {
                        foundUniqueDirectories.Add(directory);
                    }
                    else
                    {
                        foundDefaultDirectories.Add(directory);
                    }
                }

                foundUniqueDirectories = foundUniqueDirectories
                    .OrderBy(x => x.FileNameNoExtension)
                    .ToList();

                foundDefaultDirectories = foundDefaultDirectories
                    .OrderBy(x => x.FileNameNoExtension)
                    .ToList();

                projectChildDirectoryAbsolutePaths = foundUniqueDirectories
                    .Union(foundDefaultDirectories)
                    .ToList();

                var projectChildFileAbsolutePaths = Directory
                    .GetFiles(containingDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
                    .Where(x => !x.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
                    .Select(x => new AbsoluteFilePathDotNet(x, false, absoluteFilePathDotNet.ProjectId))
                    .ToList();

                return projectChildDirectoryAbsolutePaths
                    .Union(projectChildFileAbsolutePaths);
            }
        }

        if (!absoluteFilePathDotNet.IsDirectory)
        {
            return Array.Empty<AbsoluteFilePathDotNet>();
        }

        var childDirectoryAbsolutePaths = Directory
            .GetDirectories(absoluteFilePathDotNet.GetAbsoluteFilePathString())
            .OrderBy(x => x)
            .Select(x => new AbsoluteFilePathDotNet(x, true, absoluteFilePathDotNet.ProjectId))
            .ToList();

        var childFileAbsolutePaths = Directory
            .GetFiles(absoluteFilePathDotNet.GetAbsoluteFilePathString())
            .OrderBy(x => x)
            .Select(x => new AbsoluteFilePathDotNet(x, false, absoluteFilePathDotNet.ProjectId))
            .ToList();

        return childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths);
    }

    private void WorkspaceExplorerTreeViewOnEnterKeyDown(TreeViewKeyboardEventDto<AbsoluteFilePathDotNet>  treeViewKeyboardEventDto)
    {
        if (!treeViewKeyboardEventDto.Item.IsDirectory)
        {
            var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

            Dispatcher.Dispatch(
                new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    treeViewKeyboardEventDto.Item,
                    FileSystemProvider,
                    CancellationToken.None)
            );
        }
        else
        {
            treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
        }
    }

    private void WorkspaceExplorerTreeViewOnSpaceKeyDown(TreeViewKeyboardEventDto<AbsoluteFilePathDotNet> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void WorkspaceExplorerTreeViewOnDoubleClick(TreeViewMouseEventDto<AbsoluteFilePathDotNet> treeViewMouseEventDto)
    {
        if (!treeViewMouseEventDto.Item.IsDirectory)
        {
            var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

            Dispatcher.Dispatch(
                new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    treeViewMouseEventDto.Item,
                    FileSystemProvider,
                    CancellationToken.None)
            );
        }
        else
        {
            treeViewMouseEventDto.ToggleIsExpanded.Invoke();
        }
    }

    private bool GetIsExpandable(AbsoluteFilePathDotNet absoluteFilePath)
    {
        return absoluteFilePath.IsDirectory ||
               absoluteFilePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION ||
               absoluteFilePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_PROJECT;
    }

    private void DispatchAddActiveDropdownKeyActionOnClick(DropdownKey fileDropdownKey)
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(fileDropdownKey));
    }

    private void InputFileDialogOnEnterKeyDownOverride((IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded) tupleArgument)
    {
        if (tupleArgument.absoluteFilePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
        {
            Dispatcher.Dispatch(new SetFolderExplorerAction(tupleArgument.absoluteFilePath));
        }
    }

    private void OpenNewCSharpProjectDialog()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _newCSharpProjectDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_newCSharpProjectDialog));
    }
    
    private void OnProjectCreatedCallback(AbsoluteFilePathDotNet absoluteFilePathDotNet)
    {
        var localSolutionExplorerState = SolutionExplorerStateWrap.Value;

        void OnStart()
        {
            
        }

        void OnEnd(Process finishedProcess)
        {
            _ = Task.Run(async () =>
            {
                var roslynWorkspaceState = RoslynWorkspaceStateWrap.Value;
                var solutionState = SolutionStateWrap.Value;
                
                if (roslynWorkspaceState.MSBuildWorkspace is null ||
                    solutionState.Solution is null)
                {
                    return;
                }

                var nextSolution = solutionState.Solution.WithProjectFilePath(absoluteFilePathDotNet.ProjectId, absoluteFilePathDotNet.GetAbsoluteFilePathString());

                var nextProjectIdToProjectMap = new Dictionary<ProjectId, IndexedProject>(solutionState.ProjectIdToProjectMap);

                var recentlyCreatedProject = nextSolution.Projects.Single(x => x.Id == absoluteFilePathDotNet.ProjectId);
                
                var indexedProject = new IndexedProject(recentlyCreatedProject, absoluteFilePathDotNet);
                
                nextProjectIdToProjectMap.Add(absoluteFilePathDotNet.ProjectId, 
                    indexedProject);
                
                Dispatcher.Dispatch(new SetSolutionStateAction(nextSolution,
                    solutionState.ProjectIdToProjectMap,
                    solutionState.FileAbsoluteFilePathToDocumentMap,
                    solutionState.FileAbsoluteFilePathToAdditionalDocumentMap));
                
                Dispatcher.Dispatch(new AddTreeViewRootsAction(_solutionExplorerTreeViewKey,
                    new List<ITreeView>
                    {
                        new TreeView<AbsoluteFilePathDotNet>(TreeViewKey.NewTreeViewKey(), absoluteFilePathDotNet)
                    }));
            });
        }

        var command = $"dotnet sln {localSolutionExplorerState.SolutionAbsoluteFilePath.GetAbsoluteFilePathString()} " +
                      $"add {absoluteFilePathDotNet.GetAbsoluteFilePathString()}";

        Dispatcher
            .Dispatch(new EnqueueProcessOnTerminalEntryAction(
                TerminalStateFacts.GeneralTerminalEntry.TerminalEntryKey,
                command,
                null,
                OnStart,
                OnEnd,
                null,
                null,
                null,
                CancellationToken.None));
    }

    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrap_StateChanged;

        base.Dispose(disposing);
    }
}