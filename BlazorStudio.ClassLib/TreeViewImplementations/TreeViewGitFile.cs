using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Git;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewGitFile : TreeViewWithType<GitFile>
{
    public TreeViewGitFile(
        GitFile gitFile,
        ICommonComponentRenderers commonComponentRenderers,
        bool isExpandable,
        bool isExpanded)
            : base(
                gitFile,
                isExpandable,
                isExpanded)
    {
        CommonComponentRenderers = commonComponentRenderers;
    }
 
    public ICommonComponentRenderers CommonComponentRenderers { get; }

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
            CommonComponentRenderers.TreeViewGitFileRendererType!,
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

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> treeViews)
    {
        // This method is meant to do nothing in this case.
    }
}