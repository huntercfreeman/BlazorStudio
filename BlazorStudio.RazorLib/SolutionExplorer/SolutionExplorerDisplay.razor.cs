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
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
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
    private IState<BlazorStudio.ClassLib.Store.SolutionCase.SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    private void LoadFile(InputFileChangeEventArgs e)
    {
    }

    private bool _isInitialized;
    private TreeViewWrapKey _inputFileTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
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

        _newCSharpProjectDialog = new DialogRecord(
            DialogKey.NewDialogKey(),
            "New C# Project",
            typeof(NewCSharpProjectDialog),
            new Dictionary<string, object?>()
            {
                {
                    nameof(NewCSharpProjectDialog.OnProjectCreatedCallback), 
                    new Action<IAbsoluteFilePath>(OnProjectCreatedCallback)
                }
            }
        );

        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            var solutionAbsoluteFilePath =
                new AbsoluteFilePathDotNet("/home/hunter/RiderProjects/TestBlazorStudio/TestBlazorStudio.sln",
                    false, 
                    null);

            Dispatcher.Dispatch(new SetSolutionExplorerAction(solutionAbsoluteFilePath, SequenceKey.NewSequenceKey()));
        }       
        
        base.OnAfterRender(firstRender);
    }

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

                    var msBuildAbsoluteFilePath = new AbsoluteFilePath(visualStudioInstance.MSBuildPath, true);
                    
                    var solution = await workspace
                        .OpenSolutionAsync(solutionExplorerState.SolutionAbsoluteFilePath
                            .GetAbsoluteFilePathString());

                    Dispatcher.Dispatch(new SetRoslynWorkspaceStateAction(workspace, visualStudioInstance, msBuildAbsoluteFilePath));
                    
                    var projects = new List<AbsoluteFilePathDotNet>();
                    
                    Dictionary<ProjectId, IndexedProject> localProjectMap = new();
                    Dictionary<AbsoluteFilePathStringValue, IndexedDocument> localFileDocumentMap = new();

                    foreach (var project in solution.Projects)
                    {
                        var projectAbsoluteFilePath = new AbsoluteFilePathDotNet(project.FilePath ?? "{null file path}", false, project.Id);
                        
                        projects.Add(projectAbsoluteFilePath);
                        localProjectMap.Add(project.Id, new IndexedProject(project, projectAbsoluteFilePath));
                        
                        foreach (Document document in project.Documents)
                        {
                            if (document.FilePath is not null)
                            {
                                var absoluteFilePath = new AbsoluteFilePathDotNet(document.FilePath, false, project.Id);
                
                                localFileDocumentMap
                                    .Add(new AbsoluteFilePathStringValue(absoluteFilePath),
                                        new IndexedDocument(document, absoluteFilePath));
                            }
                        }
                    }
        
                    Dispatcher.Dispatch(new SetSolutionStateAction(solution,
                        localProjectMap.ToImmutableDictionary(),
                        localFileDocumentMap.ToImmutableDictionary()));
                    
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

    private void WorkspaceExplorerTreeViewOnEnterKeyDown(AbsoluteFilePathDotNet absoluteFilePath, Action toggleIsExpanded)
    {
        if (!absoluteFilePath.IsDirectory)
        {
            var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

            Dispatcher.Dispatch(
                new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    absoluteFilePath,
                    FileSystemProvider,
                    CancellationToken.None)
            );
        }
        else
        {
            toggleIsExpanded.Invoke();
        }
    }

    private void WorkspaceExplorerTreeViewOnSpaceKeyDown(AbsoluteFilePathDotNet absoluteFilePath, Action toggleIsExpanded)
    {
        toggleIsExpanded.Invoke();
    }

    private void WorkspaceExplorerTreeViewOnDoubleClick(AbsoluteFilePathDotNet absoluteFilePath, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    {
        if (!absoluteFilePath.IsDirectory)
        {
            var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

            Dispatcher.Dispatch(
                new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    absoluteFilePath,
                    FileSystemProvider,
                    CancellationToken.None)
            );
        }
        else
        {
            toggleIsExpanded.Invoke();
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
    
    private void OnProjectCreatedCallback(IAbsoluteFilePath absoluteFilePath)
    {
        var localSolutionExplorerState = SolutionExplorerStateWrap.Value;

        void OnStart()
        {
            
        }

        void OnEnd(Process finishedProcess)
        {
            
        }

        var command = $"dotnet sln {localSolutionExplorerState.SolutionAbsoluteFilePath.GetAbsoluteFilePathString()} " +
                      $"add {absoluteFilePath.GetAbsoluteFilePathString()}";

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
        //AssemblyLoadContext.Default.Resolving -= DefaultOnResolving;

        base.Dispose(disposing);
    }
}