using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.ComponentRenderers.Types;
using BlazorStudio.ClassLib.DotNet.CSharp;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Nuget;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewCSharpProjectToProjectReference : TreeViewWithType<CSharpProjectToProjectReference>
{
    public TreeViewCSharpProjectToProjectReference(
        CSharpProjectToProjectReference cSharpProjectToProjectReference,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                cSharpProjectToProjectReference,
                isExpandable,
                isExpanded)
    {
        BlazorStudioComponentRenderers = blazorStudioComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }
 
    public IBlazorStudioComponentRenderers BlazorStudioComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewCSharpProjectToProjectReference treeViewCSharpProjectToProjectReference ||
            Item is null)
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return Item?
            .GetHashCode()
               ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            BlazorStudioComponentRenderers.TreeViewCSharpProjectToProjectReferenceRendererType!,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewCSharpProjectToProjectReferenceRendererType.CSharpProjectToProjectReference),
                    Item
                },
            });
    }
    
    public override async Task LoadChildrenAsync()
    {
        if (Item is null)
            return;
        
        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}