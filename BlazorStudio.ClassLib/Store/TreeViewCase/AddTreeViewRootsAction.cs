namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public record AddTreeViewRootsAction(TreeViewWrapKey TreeViewWrapKey, List<ITreeView> RootTreeViews);