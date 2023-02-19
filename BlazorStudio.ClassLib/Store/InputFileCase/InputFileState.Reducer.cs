using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

public partial record InputFileState
{
    private static class Reducer
    {
        [ReducerMethod]
        public static InputFileState ReduceStartInputFileStateFormAction(
            InputFileState inInputFileState,
            StartInputFileStateFormAction startInputFileStateFormAction)
        {
            return inInputFileState with
            {
                SelectionIsValidFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.SelectionIsValidFunc,
                OnAfterSubmitFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.OnAfterSubmitFunc,
                InputFilePatterns = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.InputFilePatterns,
                SelectedInputFilePattern = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.InputFilePatterns
                    .First(),
                Message = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.Message
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
            if (setOpenedTreeViewModelAction.TreeViewModel.Item.IsDirectory)
            {
                return NewOpenedTreeViewModelHistory(
                    inInputFileState,
                    setOpenedTreeViewModelAction.TreeViewModel,
                    setOpenedTreeViewModelAction.CommonComponentRenderers,
                    setOpenedTreeViewModelAction.FileSystemProvider,
                    setOpenedTreeViewModelAction.EnvironmentProvider);
            }
                
            return inInputFileState;
        }
        
        [ReducerMethod]
        public static InputFileState ReduceSetSelectedInputFilePatternAction(
            InputFileState inInputFileState,
            SetSelectedInputFilePatternAction setSelectedInputFilePatternAction)
        {
            return inInputFileState with
            {
                SelectedInputFilePattern = 
                    setSelectedInputFilePatternAction.InputFilePattern
            };
        }

        [ReducerMethod]
        public static InputFileState ReduceMoveBackwardsInHistoryAction(
            InputFileState inInputFileState,
            MoveBackwardsInHistoryAction moveBackwardsInHistoryAction)
        {
            if (inInputFileState.CanMoveBackwardsInHistory)
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
            if (inInputFileState.CanMoveForwardsInHistory)
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

            TreeViewAbsoluteFilePath? parentDirectoryTreeViewModel = null;
            
            // If has a ParentDirectory select it
            if (currentSelection.Item.Directories.Any())
            {
                var parentDirectoryAbsoluteFilePath = 
                    currentSelection.Item.Directories.Last();
                
                parentDirectoryTreeViewModel = new TreeViewAbsoluteFilePath(
                    (IAbsoluteFilePath)parentDirectoryAbsoluteFilePath, 
                    openParentDirectoryAction.CommonComponentRenderers,
                    openParentDirectoryAction.FileSystemProvider,
                    openParentDirectoryAction.EnvironmentProvider,
                    false,
                    true);

                parentDirectoryTreeViewModel.LoadChildrenAsync().Wait();  
            }

            if (parentDirectoryTreeViewModel is not null)
            {
                return NewOpenedTreeViewModelHistory(
                    inInputFileState,
                    parentDirectoryTreeViewModel,
                    openParentDirectoryAction.CommonComponentRenderers,
                    openParentDirectoryAction.FileSystemProvider,
                    openParentDirectoryAction.EnvironmentProvider);
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

            currentSelection.LoadChildrenAsync().Wait();

            return inInputFileState;
        }
        
        [ReducerMethod]
        public static InputFileState ReduceSetSearchQueryAction(
            InputFileState inInputFileState,
            SetSearchQueryAction setSearchQueryAction)
        {
            var openedTreeViewModel = inInputFileState
                .OpenedTreeViewModelHistory[
                    inInputFileState.IndexInHistory];
            
            foreach (var treeViewModel in openedTreeViewModel.Children)
            {
                var treeViewAbsoluteFilePath = (TreeViewAbsoluteFilePath)treeViewModel;
                
                treeViewModel.IsHidden = !treeViewAbsoluteFilePath.Item.FilenameWithExtension
                    .Contains(
                        setSearchQueryAction.SearchQuery, 
                        StringComparison.InvariantCultureIgnoreCase);
            }

            return inInputFileState with
            {
                SearchQuery = setSearchQueryAction.SearchQuery
            };
        }
    }
}