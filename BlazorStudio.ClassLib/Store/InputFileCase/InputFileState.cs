using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.TreeView;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

[FeatureState]
public record InputFileState(
    TreeViewModel<IAbsoluteFilePath> FileSystemTreeViewModel,
    int IndexInHistory,
    ImmutableList<TreeViewModel<IAbsoluteFilePath>> OpenedTreeViewModelHistory,
    TreeViewModel<IAbsoluteFilePath>? SelectedTreeViewModel,
    Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc,
    Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc)
{
    private InputFileState() : this(
        new TreeViewModel<IAbsoluteFilePath>(
            new AbsoluteFilePath(string.Empty, true),
            true,
            LoadTreeViewRoot),
        0,
        ImmutableList<TreeViewModel<IAbsoluteFilePath>>.Empty,
        null,
        _ => Task.CompletedTask,
        _ => Task.FromResult(false))
    {
        FileSystemTreeViewModel.LoadChildrenFuncAsync
            .Invoke(FileSystemTreeViewModel);

        var firstChild = FileSystemTreeViewModel.Children.First();
        
        var selection = new TreeViewModel<IAbsoluteFilePath>(
            firstChild.Item,
            false,
            LoadNotExpandableChildrenAsync);

        selection.LoadChildrenFuncAsync.Invoke(selection);
        
        selection.IsExpanded = true;

        OpenedTreeViewModelHistory = new []
        {
            selection,
        }.ToImmutableList();
    }

    public record RequestInputFileStateFormAction(
        Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc,
        Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc);
    
    public record SetSelectedTreeViewModelAction(
        TreeViewModel<IAbsoluteFilePath>? SelectedTreeViewModel);
    
    public record SetOpenedTreeViewModelAction(
        TreeViewModel<IAbsoluteFilePath> SelectedTreeViewModel);

    public record MoveBackwardsInHistoryAction;
    public record MoveForwardsInHistoryAction;
    public record OpenParentDirectoryAction;
    public record RefreshCurrentSelectionAction;

    public static bool CanMoveBackwardsInHistory(InputFileState inputFileState) => 
        inputFileState.IndexInHistory > 0;
    
    public static bool CanMoveForwardsInHistory(InputFileState inputFileState) => 
        inputFileState.IndexInHistory < 
        inputFileState.OpenedTreeViewModelHistory.Count - 1;
    
    private class InputFileStateReducer
    {
        [ReducerMethod]
        public static InputFileState ReduceStartInputFileStateFormAction(
            InputFileState inInputFileState,
            InputFileStateEffects.StartInputFileStateFormAction startInputFileStateFormAction)
        {
            return inInputFileState with
            {
                SelectionIsValidFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.SelectionIsValidFunc,
                OnAfterSubmitFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.OnAfterSubmitFunc
            };
        }
        
        [ReducerMethod]
        public static InputFileState ReduceSetSelectedTreeViewModelAction(
            InputFileState inInputFileState,
            SetSelectedTreeViewModelAction setSelectedTreeViewModelAction)
        {
            return inInputFileState with
            {
                SelectedTreeViewModel = 
                    setSelectedTreeViewModelAction.SelectedTreeViewModel
            };
        }
        
        [ReducerMethod]
        public static InputFileState ReduceSetOpenedTreeViewModelAction(
            InputFileState inInputFileState,
            SetOpenedTreeViewModelAction setOpenedTreeViewModelAction)
        {
            return NewOpenedTreeViewModelHistory(
                inInputFileState,
                setOpenedTreeViewModelAction.SelectedTreeViewModel);
        }

        [ReducerMethod]
        public static InputFileState ReduceMoveBackwardsInHistoryAction(
            InputFileState inInputFileState,
            MoveBackwardsInHistoryAction moveBackwardsInHistoryAction)
        {
            if (CanMoveBackwardsInHistory(inInputFileState))
            {
                return inInputFileState with
                {
                    IndexInHistory = inInputFileState.IndexInHistory - 
                                             1,
                };
            }
            
            return inInputFileState;
        }
        
        [ReducerMethod]
        public static InputFileState ReduceMoveForwardsInHistoryAction(
            InputFileState inInputFileState,
            MoveForwardsInHistoryAction moveForwardsInHistoryAction)
        {
            if (CanMoveForwardsInHistory(inInputFileState))
            {
                return inInputFileState with
                {
                    IndexInHistory = inInputFileState.IndexInHistory + 
                                             1,
                };
            }
            
            return inInputFileState;
        }
        
        [ReducerMethod]
        public static InputFileState ReduceOpenParentDirectoryAction(
            InputFileState inInputFileState,
            OpenParentDirectoryAction openParentDirectoryAction)
        {
            var currentSelection = inInputFileState
                .OpenedTreeViewModelHistory[inInputFileState.IndexInHistory];

            TreeViewModel<IAbsoluteFilePath>? parentDirectoryTreeViewModel = null;
            
            // If has a ParentDirectory select it
            if (currentSelection.Item.Directories.Any())
            {
                var parentDirectoryAbsoluteFilePath = 
                    currentSelection.Item.Directories.Last();
                
                parentDirectoryTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
                    (IAbsoluteFilePath)parentDirectoryAbsoluteFilePath, 
                    false,
                    LoadNotExpandableChildrenAsync);

                parentDirectoryTreeViewModel.LoadChildrenFuncAsync
                    .Invoke(parentDirectoryTreeViewModel);  
            }

            if (parentDirectoryTreeViewModel is not null)
            {
                return NewOpenedTreeViewModelHistory(
                    inInputFileState,
                    parentDirectoryTreeViewModel);
            }

            return inInputFileState;
        }
        
        [ReducerMethod]
        public static InputFileState ReduceRefreshCurrentSelectionAction(
            InputFileState inInputFileState,
            RefreshCurrentSelectionAction refreshCurrentSelectionAction)
        {
            var currentSelection = inInputFileState
                .OpenedTreeViewModelHistory[inInputFileState.IndexInHistory];

            currentSelection.Children.Clear();

            currentSelection.LoadChildrenFuncAsync
                .Invoke(currentSelection);

            return inInputFileState;
        }
        
        private static InputFileState NewOpenedTreeViewModelHistory(
            InputFileState inInputFileState,
            TreeViewModel<IAbsoluteFilePath> selectedTreeViewModel)
        {
            var selectionClone = new TreeViewModel<IAbsoluteFilePath>(
                selectedTreeViewModel.Item,
                true,
                LoadNotExpandableChildrenAsync);

            selectionClone.LoadChildrenFuncAsync.Invoke(selectionClone);

            selectionClone.IsExpanded = true;

            var nextHistory = 
                inInputFileState.OpenedTreeViewModelHistory;
             
            // If not at end of history the more recent history is
            // replaced by the to be selected TreeViewModel
            if (inInputFileState.IndexInHistory != 
                inInputFileState.OpenedTreeViewModelHistory.Count - 1)
            {
                var historyCount = inInputFileState.OpenedTreeViewModelHistory.Count;
                var startingIndexToRemove = inInputFileState.IndexInHistory + 1;
                var countToRemove = historyCount - startingIndexToRemove;

                nextHistory = inInputFileState.OpenedTreeViewModelHistory
                    .RemoveRange(startingIndexToRemove, countToRemove);
            }
            
            nextHistory = nextHistory
                .Add(selectionClone);
            
            return inInputFileState with
            {
                IndexInHistory = inInputFileState.IndexInHistory + 1,
                OpenedTreeViewModelHistory = nextHistory,
            };
        }
    }
    
    private class InputFileStateEffects
    {
        private readonly ICommonComponentRenderers _commonComponentRenderers;

        public InputFileStateEffects(
            ICommonComponentRenderers commonComponentRenderers)
        {
            _commonComponentRenderers = commonComponentRenderers;
        }
        
        public record StartInputFileStateFormAction(
            RequestInputFileStateFormAction RequestInputFileStateFormAction);
        
        [EffectMethod]
        public Task ReduceRequestInputFileStateFormAction(
            RequestInputFileStateFormAction requestInputFileStateFormAction,
            IDispatcher dispatcher)
        {
            dispatcher.Dispatch(
                new StartInputFileStateFormAction(
                    requestInputFileStateFormAction));

            var inputFileDialog = new DialogRecord(
                DialogFacts.InputFileDialogKey,
                "Input File",
                _commonComponentRenderers.InputFileRendererType,
                null); 
            
            dispatcher.Dispatch(
                new RegisterDialogRecordAction(
                    inputFileDialog));

            return Task.CompletedTask;
        }
    }
    
    private static Task LoadTreeViewRoot(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        // HOME
        var homeAbsoluteFilePath = new AbsoluteFilePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true);
        
        var homeTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            homeAbsoluteFilePath, 
            true,
            LoadNotExpandableChildrenAsync);

        homeTreeViewModel.LoadChildrenFuncAsync.Invoke(homeTreeViewModel);    
            
        // ROOT
        var rootAbsoluteFilePath = new AbsoluteFilePath(
            "/",
            true);
        
        var rootTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            rootAbsoluteFilePath, 
            true,
            LoadExpandableChildrenAsync);

        rootTreeViewModel.LoadChildrenFuncAsync.Invoke(rootTreeViewModel);
        
        treeViewModel.Children.Clear();
        
        treeViewModel.Children.AddRange(new []
        {
            homeTreeViewModel,
            rootTreeViewModel
        });

        return Task.CompletedTask;
    }

    private static async Task LoadExpandableChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var children = await ReadFilesAsync(treeViewModel);

        var childTreeViewModels = children
            .Select(afp => 
                new TreeViewModel<IAbsoluteFilePath>(
                    afp,
                    true,
                    LoadNotExpandableChildrenAsync));

        treeViewModel.Children.AddRange(childTreeViewModels);
    }
    
    private static async Task LoadNotExpandableChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var children = await ReadFilesAsync(treeViewModel);

        var childTreeViewModels = children
            .Select(afp => 
                new TreeViewModel<IAbsoluteFilePath>(
                    afp,
                    false,
                    LoadNotExpandableChildrenAsync));

        treeViewModel.Children.AddRange(childTreeViewModels);
    }
    
    private static async Task<IEnumerable<AbsoluteFilePath>> ReadFilesAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var absoluteFilePathString = treeViewModel.Item.GetAbsoluteFilePathString();

        var childFiles = Directory
            .GetFiles(absoluteFilePathString)
            .OrderBy(filename => filename)
            .Select(cf => new AbsoluteFilePath(cf, false));
        
        var childDirectories = Directory
            .GetDirectories(absoluteFilePathString)
            .OrderBy(filename => filename)
            .Select(cd => new AbsoluteFilePath(cd, true));

        return childDirectories
            .Union(childFiles);
    }
}