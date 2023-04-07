using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.ComponentRenderers.Types;
using BlazorStudio.ClassLib.Git;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewGitFile : TreeViewWithType<GitFile>
{
    public TreeViewGitFile(
        GitFile gitFile,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        bool isExpandable,
        bool isExpanded)
            : base(
                gitFile,
                isExpandable,
                isExpanded)
    {
        BlazorStudioComponentRenderers = blazorStudioComponentRenderers;
    }
 
    public IBlazorStudioComponentRenderers BlazorStudioComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewGitFile treeViewGitFile ||
            treeViewGitFile.Item is null ||
            Item is null)
        {
            return false;
        }

        return treeViewGitFile.Item.AbsoluteFilePath
                   .GetAbsoluteFilePathString() == 
               Item.AbsoluteFilePath.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item?.AbsoluteFilePath.GetAbsoluteFilePathString().GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            BlazorStudioComponentRenderers.TreeViewGitFileRendererType!,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewGitFileRendererType.TreeViewGitFile),
                    this
                },
            });
    }
    
    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        // This method is meant to do nothing in this case.
    }
}