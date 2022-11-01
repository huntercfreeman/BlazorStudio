using BlazorStudio.ClassLib.TreeView;

namespace BlazorStudio.RazorLib.TreeView;

public class InternalParameters<TItem>
{
    internal Func<TreeViewModel<TItem>>? GetRootFunc { get; set; }
    internal int Depth { get; set; }
    internal int Index { get; set; }
    internal TreeViewDisplay<TItem>? ParentTreeViewDisplay { get; set; }
}