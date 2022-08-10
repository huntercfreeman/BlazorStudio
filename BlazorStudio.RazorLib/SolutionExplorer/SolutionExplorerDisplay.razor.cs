using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
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
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Store.SolutionCase;
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
    private TreeViewWrap<IAbsoluteFilePath> _treeViewWrap = null!;
    private List<IAbsoluteFilePath> _rootAbsoluteFilePaths;
    private RichErrorModel? _solutionExplorerStateWrapStateChangedRichErrorModel;
    private TreeViewWrapDisplay<IAbsoluteFilePath>? _treeViewWrapDisplay;
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
    private Solution? _solution;
    private bool _loadingSln;
    private MSBuildWorkspace _workspace;
    private VisualStudioInstance _visualStudioInstance;

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

    private async void SolutionExplorerStateWrap_StateChanged(object? sender, EventArgs e)
    {
        var solutionExplorerState = SolutionExplorerStateWrap.Value;

        if (solutionExplorerState.SolutionAbsoluteFilePath is not null)
        {
            _isInitialized = false;
            _solutionExplorerStateWrapStateChangedRichErrorModel = null;
            _rootAbsoluteFilePaths = null;

            await InvokeAsync(StateHasChanged);

            _treeViewWrap = new TreeViewWrap<IAbsoluteFilePath>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
            {
                _rootAbsoluteFilePaths = (await LoadAbsoluteFilePathChildrenAsync(solutionExplorerState.SolutionAbsoluteFilePath))
                    .ToList();

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

    private async Task<IEnumerable<IAbsoluteFilePath>> LoadAbsoluteFilePathChildrenAsync(IAbsoluteFilePath absoluteFilePath)
    {
        if (absoluteFilePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
        {
            try
            {
                _loadingSln = true;

                await InvokeAsync(StateHasChanged);

                var targetPath = absoluteFilePath.GetAbsoluteFilePathString();

                // Attempt to set the version of MSBuild.
                VisualStudioInstance[] visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

                // TODO: Allow user to select the MSBuild
                _visualStudioInstance = visualStudioInstances[0];

                if (!MSBuildLocator.IsRegistered)
                {
                    MSBuildLocator.RegisterInstance(_visualStudioInstance);

                    //var instance = Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();

                    //foreach (var assemblyLoadContext in AssemblyLoadContext.All)
                    //{
                    //    assemblyLoadContext.Resolving += (innerAssemblyLoadContext, assemblyName) =>
                    //    {
                    //        var path = Path.Combine(instance.MSBuildPath, assemblyName.Name + ".dll");
                    //        if (File.Exists(path))
                    //        {
                    //            return innerAssemblyLoadContext.LoadFromAssemblyPath(path);
                    //        }

                    //        return null;
                    //    };
                    //}

                    //AssemblyLoadContext.Default.Resolving += DefaultOnResolving;
                    //var previewPath = "C:\\Program Files\\dotnet\\sdk\\7.0.100-preview.6.22352.1\\";

                    //if (Directory.Exists(previewPath))
                    //{
                    //    MSBuildLocator.RegisterMSBuildPath(previewPath);
                    //}
                    //else
                    //{
                    //    MSBuildLocator.RegisterInstance(_visualStudioInstance);
                    //}

                    //var instance = Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
                }

                if (_workspace is null)
                {
                    _workspace = MSBuildWorkspace.Create();

                    Dispatcher.Dispatch(new SetSolutionAction(_workspace, _visualStudioInstance));
                }

                // Print message for WorkspaceFailed event to help diagnosing project load failures.

                _solution = await _workspace.OpenSolutionAsync(targetPath);

                var projects = new List<AbsoluteFilePath>();

                foreach (var project in _solution.Projects)
                {
                    projects.Add(new AbsoluteFilePath(project.FilePath ?? "{null file path}", false));
                }

                _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
                    {
                        await FileToDocumentIndexer.IndexFilesToDocuments(_workspace, Dispatcher, cancellationToken);
                    },
                    $"{nameof(FileToDocumentIndexer.IndexFilesToDocuments)}",
                    false,
                    TimeSpan.FromSeconds(60));

                return projects.ToArray();
            }
            finally
            {
                _loadingSln = false;
            }
        }
        
        if (absoluteFilePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_PROJECT)
        {
            var containingDirectory = absoluteFilePath.Directories.Last();

            if (containingDirectory is AbsoluteFilePath containingDirectoryAbsoluteFilePath)
            {
                var hiddenFiles = HiddenFileFacts
                    .GetHiddenFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

                var projectChildDirectoryAbsolutePaths = Directory
                    .GetDirectories(containingDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
                    .Where(x => hiddenFiles.All(hidden => !x.EndsWith(hidden)))
                    .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
                    .ToList();

                var uniqueDirectories =
                    UniqueFileFacts.GetUniqueFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

                var foundUniqueDirectories = new List<IAbsoluteFilePath>();
                var foundDefaultDirectories = new List<IAbsoluteFilePath>();

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
                    .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
                    .ToList();

                return projectChildDirectoryAbsolutePaths
                    .Union(projectChildFileAbsolutePaths);
            }
        }

        if (!absoluteFilePath.IsDirectory)
        {
            return Array.Empty<IAbsoluteFilePath>();
        }

        var childDirectoryAbsolutePaths = Directory
            .GetDirectories(absoluteFilePath.GetAbsoluteFilePathString())
            .OrderBy(x => x)
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
            .ToList();

        var childFileAbsolutePaths = Directory
            .GetFiles(absoluteFilePath.GetAbsoluteFilePathString())
            .OrderBy(x => x)
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
            .ToList();

        return childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths);
    }

    /// <summary>
    /// This is not working
    ///     "8: The "ProcessFrameworkReferences" task failed unexpectedly. ["
    ///     "4018: System.IO.FileLoadException: Could not load file or assembly 'NuGet.Frameworks, Version=6.3.0.114, Culture=n"
    /// </summary>
    /// <param name="assemblyLoadContext"></param>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    private Assembly? DefaultOnResolving(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
    {
        var path = Path.Combine(_visualStudioInstance.MSBuildPath, assemblyName.Name + ".dll");
        if (File.Exists(path))
        {
            return assemblyLoadContext.LoadFromAssemblyPath(path);
        }

        return null;
    }

    private void WorkspaceExplorerTreeViewOnEnterKeyDown(IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded)
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

    private void WorkspaceExplorerTreeViewOnSpaceKeyDown(IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded)
    {
        toggleIsExpanded.Invoke();
    }

    private void WorkspaceExplorerTreeViewOnDoubleClick(IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
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

    private bool GetIsExpandable(IAbsoluteFilePath absoluteFilePath)
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
            Dispatcher.Dispatch(new SetWorkspaceAction(tupleArgument.absoluteFilePath));
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
            _workspace.CloseSolution();

            SolutionExplorerStateWrap_StateChanged(null, EventArgs.Empty);
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