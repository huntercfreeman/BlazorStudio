﻿using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor.Blazor.Web.Components;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.DialogCase;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerDisplay : FluxorComponent, IDisposable
{
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
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

    private DropdownKey _fileDropdownKey = DropdownKey.NewDropdownKey();
    private Solution? _sln;
    private bool _loadingSln;

    protected override void OnInitialized()
    {
        SolutionExplorerStateWrap.StateChanged += SolutionExplorerStateWrap_StateChanged;

        base.OnInitialized();
    }

    private async void SolutionExplorerStateWrap_StateChanged(object? sender, EventArgs e)
    {
        var workspaceState = SolutionExplorerStateWrap.Value;

        if (workspaceState.SolutionAbsoluteFilePath is not null)
        {
            _isInitialized = false;
            _solutionExplorerStateWrapStateChangedRichErrorModel = null;
            _rootAbsoluteFilePaths = null;

            await InvokeAsync(StateHasChanged);

            _treeViewWrap = new TreeViewWrap<IAbsoluteFilePath>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
            {
                _rootAbsoluteFilePaths = (await LoadAbsoluteFilePathChildrenAsync(workspaceState.SolutionAbsoluteFilePath))
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
        if (absoluteFilePath.ExtensionNoPeriod == "sln")
        {
            try
            {
                _loadingSln = true;

                await InvokeAsync(StateHasChanged);

                var targetPath = absoluteFilePath.GetAbsoluteFilePathString();

                MSBuildLocator.RegisterDefaults();

                var workspace = MSBuildWorkspace.Create();

                _sln = await workspace.OpenSolutionAsync(targetPath);

                var projects = new List<AbsoluteFilePath>();

                foreach (var project in _sln.Projects)
                {
                    projects.Add(new AbsoluteFilePath(project.FilePath ?? "{null file path}", false));
                }


                return projects.ToArray();
            }
            finally
            {
                _loadingSln = false;
            }
        }
        
        if (absoluteFilePath.ExtensionNoPeriod == "csproj")
        {
            var containingDirectory = absoluteFilePath.Directories.Last();

            if (containingDirectory is AbsoluteFilePath containingDirectoryAbsoluteFilePath)
            {
                var projectChildDirectoryAbsolutePaths = Directory
                    .GetDirectories(containingDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
                    .Where(x => !x.EndsWith("bin") && !x.EndsWith("obj"))
                    .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
                    .ToList();

                var projectChildFileAbsolutePaths = Directory
                    .GetFiles(containingDirectoryAbsoluteFilePath.GetAbsoluteFilePathString())
                    .Where(x => !x.EndsWith("csproj"))
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

#if DEBUG
        var childDirectoryAbsolutePaths = Directory
            .GetDirectories(absoluteFilePath.GetAbsoluteFilePathString())
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
            .ToList();

        var childFileAbsolutePaths = Directory
            .GetFiles(absoluteFilePath.GetAbsoluteFilePathString())
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
            .ToList();

        return childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths);
#else
        var childDirectoryAbsolutePaths = new string[]
            {
                "Dir1",
                "Dir2",
                "Dir3",
            }
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
            .ToList();

        var childFileAbsolutePaths = new string[]
            {
                "File1",
                "File2",
                "File3",
            }
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
            .ToList();

        return childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths);
#endif
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
                new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
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
               absoluteFilePath.ExtensionNoPeriod == "sln" ||
               absoluteFilePath.ExtensionNoPeriod == "csproj";
    }

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewWrapDisplay<IAbsoluteFilePath>.ContextMenuEventDto<IAbsoluteFilePath> contextMenuEventDto)
    {
        var createNewFile = MenuOptionFacts.File
            .ConstructCreateNewFile(typeof(CreateNewFileForm),
                new Dictionary<string, object?>()
                {
                    {
                        nameof(CreateNewFileForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterSubmitForm),
                        new Action<string, string>(CreateNewFileFormOnAfterSubmitForm)
                    },
                });

        var createNewDirectory = MenuOptionFacts.File
            .ConstructCreateNewDirectory(typeof(CreateNewDirectoryForm),
                new Dictionary<string, object?>()
                {
                    {
                        nameof(CreateNewFileForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterSubmitForm),
                        new Action<string, string>(CreateNewDirectoryFormOnAfterSubmitForm)
                    },
                });

        _mostRecentRefreshContextMenuTarget = contextMenuEventDto.RefreshContextMenuTarget;

        List<MenuOptionRecord> menuOptionRecords = new();

        if (contextMenuEventDto.Item.IsDirectory)
        {
            menuOptionRecords.Add(createNewFile);
            menuOptionRecords.Add(createNewDirectory);
        }

        return menuOptionRecords.Any()
            ? menuOptionRecords
            : new[]
            {
                new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                    "No Context Menu Options for this item",
                    ImmutableList<MenuOptionRecord>.Empty,
                    null)
            };
    }

    private void CreateNewFileFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string fileName)
    {
#if DEBUG
        var localRefreshContextMenuTarget = _mostRecentRefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
        {
            await File
                .AppendAllTextAsync(parentDirectoryAbsoluteFilePathString + fileName,
                    string.Empty);

            await localRefreshContextMenuTarget();

            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
        },
            $"{nameof(CreateNewFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
#endif
    }

    private void CreateNewDirectoryFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string directoryName)
    {
#if DEBUG
        var localRefreshContextMenuTarget = _mostRecentRefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
        {
            Directory.CreateDirectory(parentDirectoryAbsoluteFilePathString + directoryName);

            await localRefreshContextMenuTarget();

            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
        },
            $"{nameof(CreateNewDirectoryFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
#endif
    }

    private void DispatchAddActiveDropdownKeyActionOnClick(DropdownKey fileDropdownKey)
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(fileDropdownKey));
    }

    private void InputFileDialogOnEnterKeyDownOverride((IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded) tupleArgument)
    {
        if (tupleArgument.absoluteFilePath.ExtensionNoPeriod == "sln")
        {
            Dispatcher.Dispatch(new SetWorkspaceAction(tupleArgument.absoluteFilePath));
        }
    }

    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrap_StateChanged;


        base.Dispose(disposing);
    }
}