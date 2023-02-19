using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> LoadChildrenForRazorMarkupAsync(
        TreeViewNamespacePath razorMarkupTreeView)
    {
        if (razorMarkupTreeView.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());
        
        var parentDirectoryOfRazorMarkup = (IAbsoluteFilePath)
            razorMarkupTreeView.Item.AbsoluteFilePath.Directories
                .Last();
        
        var parentAbsoluteFilePathString = parentDirectoryOfRazorMarkup
            .GetAbsoluteFilePathString();

        var childFileTreeViewModels = 
            
            razorMarkupTreeView.FileSystemProvider
                .DirectoryGetFiles(parentAbsoluteFilePathString)
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(
                        x,
                        false,
                        razorMarkupTreeView.EnvironmentProvider);

                    var namespaceString = razorMarkupTreeView.Item.Namespace;
                    
                    return (TreeViewNoType)new TreeViewNamespacePath(
                        new NamespacePath(
                            namespaceString,
                            absoluteFilePath),
                        razorMarkupTreeView.CommonComponentRenderers,
                        razorMarkupTreeView.SolutionExplorerStateWrap,
                        razorMarkupTreeView.FileSystemProvider,
                        razorMarkupTreeView.EnvironmentProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                    };
                }).ToList();

        RazorMarkupFindRelatedFiles(
            razorMarkupTreeView,
            childFileTreeViewModels);

        return Task.FromResult(razorMarkupTreeView.Children);
    }
    
    public static void RazorMarkupFindRelatedFiles(
        TreeViewNamespacePath razorMarkupTreeView,
        List<TreeViewNoType> treeViews)
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