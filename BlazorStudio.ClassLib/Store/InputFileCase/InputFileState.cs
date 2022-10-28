using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TreeView;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

[FeatureState]
public record InputFileState(
    TreeViewModel<IAbsoluteFilePath> FileSystemTreeViewModel,
    int SelectedIndexInHistory,
    ImmutableList<TreeViewModel<IAbsoluteFilePath>> SelectedTreeViewModelHistory)
{
    private InputFileState() : this(
        new TreeViewModel<IAbsoluteFilePath>(
            new AbsoluteFilePath(string.Empty, true),
            LoadTreeViewRoot),
        0,
        ImmutableList<TreeViewModel<IAbsoluteFilePath>>.Empty)
    {
        FileSystemTreeViewModel.LoadChildrenFuncAsync
            .Invoke(FileSystemTreeViewModel);

        var firstChild = FileSystemTreeViewModel.Children.First();
        
        var selection = new TreeViewModel<IAbsoluteFilePath>(
            firstChild.Item,
            LoadChildrenAsync);

        selection.LoadChildrenFuncAsync.Invoke(selection);
        
        selection.IsExpanded = true;

        SelectedTreeViewModelHistory = new []
        {
            selection,
        }.ToImmutableList();
    }

    public record SetSelectedTreeViewModelAction(
        TreeViewModel<IAbsoluteFilePath> SelectedTreeViewModel);

    public record SelectPreviousHistoryIndexAction;
    public record SelectNextHistoryIndexAction;
    public record SelectParentDirectoryAction;
    public record RefreshCurrentSelectionAction;
    
    private class InputFileStateReducer
    {
        [ReducerMethod]
        public static InputFileState ReduceSetSelectedTreeViewModel(
            InputFileState inInputFileState,
            SetSelectedTreeViewModelAction setSelectedTreeViewModelAction)
        {
            return SelectNewHistory(
                inInputFileState,
                setSelectedTreeViewModelAction.SelectedTreeViewModel);
        }

        [ReducerMethod]
        public static InputFileState ReduceSelectPreviousHistoryIndexAction(
            InputFileState inInputFileState,
            SelectPreviousHistoryIndexAction selectPreviousHistoryIndexAction)
        {
            var nextSelectedIndexInHistory = inInputFileState.SelectedIndexInHistory; 
            
            if (nextSelectedIndexInHistory > 0)
                nextSelectedIndexInHistory--;
            
            return inInputFileState with
            {
                SelectedIndexInHistory = nextSelectedIndexInHistory,
            };
        }
        
        [ReducerMethod]
        public static InputFileState ReduceSelectNextHistoryIndexAction(
            InputFileState inInputFileState,
            SelectNextHistoryIndexAction selectNextHistoryIndexAction)
        {
            var nextSelectedIndexInHistory = inInputFileState.SelectedIndexInHistory; 
            
            if (nextSelectedIndexInHistory < inInputFileState.SelectedTreeViewModelHistory.Count - 1)
                nextSelectedIndexInHistory--;
            
            return inInputFileState with
            {
                SelectedIndexInHistory = nextSelectedIndexInHistory,
            };
        }
        
        [ReducerMethod]
        public static InputFileState ReduceSelectParentDirectoryAction(
            InputFileState inInputFileState,
            SelectParentDirectoryAction selectParentDirectoryAction)
        {
            var currentSelection = inInputFileState
                .SelectedTreeViewModelHistory[inInputFileState.SelectedIndexInHistory];

            TreeViewModel<IAbsoluteFilePath>? parentDirectoryTreeViewModel = null;
            
            // If has a ParentDirectory select it
            if (currentSelection.Item.Directories.Any())
            {
                var parentDirectoryAbsoluteFilePath = 
                    currentSelection.Item.Directories.Last();
                
                parentDirectoryTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
                    (IAbsoluteFilePath)parentDirectoryAbsoluteFilePath, 
                    LoadChildrenAsync);

                parentDirectoryTreeViewModel.LoadChildrenFuncAsync
                    .Invoke(parentDirectoryTreeViewModel);  
            }

            if (parentDirectoryTreeViewModel is not null)
            {
                return SelectNewHistory(
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
                .SelectedTreeViewModelHistory[inInputFileState.SelectedIndexInHistory];

            currentSelection.Children.Clear();

            currentSelection.LoadChildrenFuncAsync
                .Invoke(currentSelection);

            return inInputFileState;
        }
        
        private static InputFileState SelectNewHistory(
            InputFileState inInputFileState,
            TreeViewModel<IAbsoluteFilePath> selectedTreeViewModel)
        {
            var selectionClone = new TreeViewModel<IAbsoluteFilePath>(
                selectedTreeViewModel.Item,
                LoadChildrenAsync);

            selectionClone.LoadChildrenFuncAsync.Invoke(selectionClone);

            selectionClone.IsExpanded = true;
            
            var nextHistory = 
                inInputFileState.SelectedTreeViewModelHistory;
             
            // If not at end of history the more recent history is
            // replaced by the to be selected TreeViewModel
            if (inInputFileState.SelectedIndexInHistory != 
                inInputFileState.SelectedTreeViewModelHistory.Count - 1)
            {
                var historyCount = inInputFileState.SelectedTreeViewModelHistory.Count;
                var startingIndexToRemove = inInputFileState.SelectedIndexInHistory + 1;
                var countToRemove = historyCount - startingIndexToRemove;

                nextHistory = inInputFileState.SelectedTreeViewModelHistory
                    .RemoveRange(startingIndexToRemove, countToRemove);
            }
            
            nextHistory = nextHistory
                .Add(selectedTreeViewModel);
            
            return inInputFileState with
            {
                SelectedIndexInHistory = inInputFileState.SelectedIndexInHistory + 1,
                SelectedTreeViewModelHistory = nextHistory,
            };
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
            LoadChildrenAsync);

        homeTreeViewModel.LoadChildrenFuncAsync.Invoke(homeTreeViewModel);    
            
        // ROOT
        var rootAbsoluteFilePath = new AbsoluteFilePath(
            "/",
            true);
        
        var rootTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            rootAbsoluteFilePath, 
            LoadChildrenAsync);

        rootTreeViewModel.LoadChildrenFuncAsync.Invoke(rootTreeViewModel);
        
        treeViewModel.Children.Clear();
        
        treeViewModel.Children.AddRange(new []
        {
            homeTreeViewModel,
            rootTreeViewModel
        });

        return Task.CompletedTask;
    }

    private static Task LoadChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
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

        var childTreeViewModels = childDirectories
            .Union(childFiles)
            .Select(afp => new TreeViewModel<IAbsoluteFilePath>(afp, LoadChildrenAsync));

        treeViewModel.Children.AddRange(childTreeViewModels);
        
        return Task.CompletedTask;
    }
}