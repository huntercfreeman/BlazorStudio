using BlazorStudio.ClassLib.Git;
using BlazorStudio.ClassLib.TreeViewImplementations;

namespace BlazorStudio.ClassLib.CommonComponents;

public interface ITreeViewGitFileRendererType
{
    public TreeViewGitFile TreeViewGitFile { get; set; }
}