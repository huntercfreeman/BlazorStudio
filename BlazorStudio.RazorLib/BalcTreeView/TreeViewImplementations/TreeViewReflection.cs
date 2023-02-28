using System.Collections;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;

namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

public class TreeViewReflection : TreeViewWithType<TextEditorDebugObjectWrap>
{
    private readonly ITreeViewRenderers _treeViewRenderers;

    public TreeViewReflection(
        TextEditorDebugObjectWrap textEditorDebugObjectWrap,
        bool isExpandable,
        bool isExpanded,
        ITreeViewRenderers treeViewRenderers)
        : base(
            textEditorDebugObjectWrap,
            isExpandable,
            isExpanded)
    {
        _treeViewRenderers = treeViewRenderers;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewReflection treeViewReflection)
        {
            return false;
        }

        return treeViewReflection.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item?.GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewRenderers.TreeViewReflectionRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewReflection),
                    this
                },
            });
    }

    public override Task LoadChildrenAsync()
    {
        if (Item is null)
            throw new ApplicationException("Node was null");
        
        var oldChildrenMap = Children
            .ToDictionary(child => child);
        
        try
        {
            Children.Clear();
            
            Children.Add(new TreeViewFields(
                Item,
                true,
                false,
                _treeViewRenderers));
            
            Children.Add(new TreeViewProperties(
                Item,
                true,
                false,
                _treeViewRenderers));
            
            if (Item.DebugObjectItem is IEnumerable)
            {
                Children.Add(new TreeViewEnumerable(Item,
                    true,
                    false,
                    _treeViewRenderers));
            }
            
            if (Item.DebugObjectItemType.IsInterface &&
                Item.DebugObjectItem is not null)
            {
                var interfaceImplementation = new TextEditorDebugObjectWrap(
                    Item.DebugObjectItem,
                    Item.DebugObjectItem.GetType(),
                    "InterfaceImplementation",
                    false);
                    
                Children.Add(new TreeViewInterfaceImplementation(
                    interfaceImplementation,
                    true,
                    false,
                    _treeViewRenderers));
            }
        }
        catch (Exception e)
        {
            Children.Clear();
            Children.Add(new TreeViewException(
                e,
                false,
                false,
                _treeViewRenderers));
        }
        
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            child.Parent = this;
            child.IndexAmongSiblings = i;
        }
        
        foreach (var newChild in Children)
        {
            if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
            {
                newChild.IsExpanded = oldChild.IsExpanded;
                newChild.IsExpandable = oldChild.IsExpandable;
                newChild.IsHidden = oldChild.IsHidden;
                newChild.TreeViewNodeKey = oldChild.TreeViewNodeKey;
                newChild.Children = oldChild.Children;
            }
        }
        
        return Task.CompletedTask;
    }
}