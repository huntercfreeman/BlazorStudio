using System.Collections.Immutable;
using BlazorStudio.ClassLib.TreeView;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

[FeatureState]
public record TreeViewStates(ImmutableDictionary<TreeViewKey, ITreeViewModel> TreeViewMap)
{
    public record RegisterTreeViewModelAction(ITreeViewModel TreeViewModel);
    public record DisposeTreeViewModelAction(TreeViewKey TreeViewKey);
    
    private class TreeViewStatesReducer
    {
        [ReducerMethod]
        public static TreeViewStates ReduceRegisterTreeViewModelAction(
            TreeViewStates inTreeViewStates,
            RegisterTreeViewModelAction registerTreeViewModelAction)
        {
            if (inTreeViewStates.TreeViewMap.ContainsKey(registerTreeViewModelAction.TreeViewModel.TreeViewKey))
                return inTreeViewStates;

            var outTreeViewMap = inTreeViewStates.TreeViewMap
                .Add(
                    registerTreeViewModelAction.TreeViewModel.TreeViewKey, 
                    registerTreeViewModelAction.TreeViewModel);

            return inTreeViewStates with
            {
                TreeViewMap = outTreeViewMap
            };
        }
        
        [ReducerMethod]
        public static TreeViewStates ReduceDisposeTreeViewModelAction(
            TreeViewStates inTreeViewStates,
            DisposeTreeViewModelAction disposeTreeViewModelAction)
        {
            var outTreeViewMap = inTreeViewStates.TreeViewMap
                .Remove(disposeTreeViewModelAction.TreeViewKey);
            
            return inTreeViewStates with
            {
                TreeViewMap = outTreeViewMap
            }; 
        }
    }
}