using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.TreeViewImplementations;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

public partial record InputFileState
{
    public bool CanMoveBackwardsInHistory => IndexInHistory > 0;
    
    public bool CanMoveForwardsInHistory => IndexInHistory < 
        OpenedTreeViewModelHistory.Count - 1;

    public TreeViewAbsoluteFilePath? GetOpenedTreeView()
    {
        if (IndexInHistory == -1 ||
            IndexInHistory < OpenedTreeViewModelHistory.Count)
            return null;
        
        return OpenedTreeViewModelHistory[IndexInHistory];
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