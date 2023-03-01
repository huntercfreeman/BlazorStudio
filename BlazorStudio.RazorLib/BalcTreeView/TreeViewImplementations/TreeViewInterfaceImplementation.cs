using BlazorCommon.RazorLib.TreeView.TreeViewClasses;

namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

/// <summary>
/// <see cref="TreeViewAdhoc"/> is used when the
/// consumer of the component does not want to show the root.
/// <br/><br/>
/// The TreeViews were designed with a root consisting of 1 node.
/// To get around this <see cref="TreeViewAdhoc"/> can be used
/// to have that top level root node be invisible to the user.
/// </summary>
public class TreeViewInterfaceImplementation : TreeViewReflection
{
    private readonly ITreeViewRenderers _treeViewRenderers;

    public TreeViewInterfaceImplementation(
        TextEditorDebugObjectWrap textEditorDebugObjectWrap,
        bool isExpandable,
        bool isExpanded,
        ITreeViewRenderers treeViewRenderers)
        : base(
            textEditorDebugObjectWrap,
            isExpandable,
            isExpanded,
            treeViewRenderers)
    {
        _treeViewRenderers = treeViewRenderers;
    }
    
    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewRenderers.TreeViewInterfaceImplementationRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewInterfaceImplementation),
                    this
                },
            });
    }
}