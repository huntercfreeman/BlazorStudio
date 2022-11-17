using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTextEditor.RazorLib.TreeView;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeView>> LoadChildrenForRazorMarkupAsync(
        TreeViewNamespacePath razorMarkupTreeView)
    {
        if (razorMarkupTreeView.Item is null)
            return new();
        
        var parentDirectoryOfRazorMarkup = (IAbsoluteFilePath)
            razorMarkupTreeView.Item.AbsoluteFilePath.Directories
                .Last();
        
        var parentAbsoluteFilePathString = parentDirectoryOfRazorMarkup
            .GetAbsoluteFilePathString();

        var childFileTreeViewModels = Directory
            .GetFiles(parentAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, false);

                var namespaceString = razorMarkupTreeView.Item.Namespace;
                
                return (TreeView)new TreeViewNamespacePath(
                    new NamespacePath(
                        namespaceString,
                        absoluteFilePath),
                    razorMarkupTreeView.CommonComponentRenderers,
                    razorMarkupTreeView.SolutionExplorerStateWrap)
                {
                    IsExpandable = false,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            }).ToList();

        RazorMarkupFindRelatedFiles(
            razorMarkupTreeView,
            childFileTreeViewModels);

        return razorMarkupTreeView.Children;
    }
    
    public static void RazorMarkupFindRelatedFiles(
        TreeViewNamespacePath razorMarkupTreeView,
        List<TreeView> treeViews)
    {
        if (razorMarkupTreeView.Item is null)
            return;

        razorMarkupTreeView.Children.Clear();
        
        // .razor files look to remove .razor.cs and .razor.css files

        var matches = new[]
        {
            razorMarkupTreeView.Item.AbsoluteFilePath.FilenameWithExtension +
                '.' + 
                ExtensionNoPeriodFacts.C_SHARP_CLASS,
            razorMarkupTreeView.Item.AbsoluteFilePath.FilenameWithExtension +
                '.' + 
                ExtensionNoPeriodFacts.CSS
        };
        
        var relatedFile = treeViews.FirstOrDefault(x =>
            x.UntypedItem is NamespacePath namespacePath &&
            matches.Contains(namespacePath.AbsoluteFilePath.FilenameWithExtension));

        if (relatedFile is null)
        {
            return;
        }

        treeViews.Remove(relatedFile);

        razorMarkupTreeView.IsExpandable = true;

        razorMarkupTreeView.Children.Add(relatedFile);
    }
}