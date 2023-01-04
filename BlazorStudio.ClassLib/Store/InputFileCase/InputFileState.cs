using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

[FeatureState]
public record InputFileState(
    int IndexInHistory,
    ImmutableList<TreeViewAbsoluteFilePath> OpenedTreeViewModelHistory,
    TreeViewAbsoluteFilePath? SelectedTreeViewModel,
    Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc,
    Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc,
    ImmutableArray<InputFilePattern> InputFilePatterns,
    InputFilePattern? SelectedInputFilePattern,
    string SearchQuery,
    string Message)
{
    private InputFileState() : this(
        -1,
        ImmutableList<TreeViewAbsoluteFilePath>.Empty,
        null,
        _ => Task.CompletedTask,
        _ => Task.FromResult(false),
        ImmutableArray<InputFilePattern>.Empty,
        null,
        string.Empty,
        string.Empty) 
    {
    }

    public record RequestInputFileStateFormAction(
        string Message,
        Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc,
        Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc,
        ImmutableArray<InputFilePattern> InputFilePatterns);
    
    public record SetSelectedTreeViewModelAction(
        TreeViewAbsoluteFilePath? SelectedTreeViewModel);
    
    public record SetOpenedTreeViewModelAction(
        TreeViewAbsoluteFilePath TreeViewModel,
        ICommonComponentRenderers CommonComponentRenderers);
    
    public record SetSelectedInputFilePatternAction(
        InputFilePattern InputFilePattern);
    
    public record SetSearchQueryAction(
        string SearchQuery);

    public record MoveBackwardsInHistoryAction;
    public record MoveForwardsInHistoryAction;
    public record OpenParentDirectoryAction(ICommonComponentRenderers CommonComponentRenderers);
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
                    setOpenedTreeViewModelAction.CommonComponentRenderers);
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

            TreeViewAbsoluteFilePath? parentDirectoryTreeViewModel = null;
            
            // If has a ParentDirectory select it
            if (currentSelection.Item.Directories.Any())
            {
                var parentDirectoryAbsoluteFilePath = 
                    currentSelection.Item.Directories.Last();
                
                parentDirectoryTreeViewModel = new TreeViewAbsoluteFilePath(
                    (IAbsoluteFilePath)parentDirectoryAbsoluteFilePath, 
                    openParentDirectoryAction.CommonComponentRenderers,
                    false,
                    true);

                parentDirectoryTreeViewModel.LoadChildrenAsync().Wait();  
            }

            if (parentDirectoryTreeViewModel is not null)
            {
                return NewOpenedTreeViewModelHistory(
                    inInputFileState,
                    parentDirectoryTreeViewModel,
                    openParentDirectoryAction.CommonComponentRenderers);
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
        
        private static InputFileState NewOpenedTreeViewModelHistory(
            InputFileState inInputFileState,
            TreeViewAbsoluteFilePath selectedTreeViewModel,
            ICommonComponentRenderers commonComponentRenderers)
        {
            var selectionClone = new TreeViewAbsoluteFilePath(
                selectedTreeViewModel.Item,
                commonComponentRenderers,
                false,
                true);

            selectionClone.LoadChildrenAsync().Wait();

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
        public Task HandleRequestInputFileStateFormAction(
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
                null)
            {
                IsResizable = true
            }; 
            
            dispatcher.Dispatch(
                new DialogsState.RegisterDialogRecordAction(
                    inputFileDialog));

            return Task.CompletedTask;
        }
    }
}