using BlazorStudio.ClassLib.TreeView;

namespace BlazorStudio.RazorLib.TreeView;

public record TreeViewDisplayInternalParameters<TItem>(
    int Depth,
    int Index,
    Func<TreeViewModel<TItem>>? GetRootFunc,
    TreeViewDisplay<TItem>? ParentTreeViewDisplay);