using System.Reflection;
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
public class TreeViewFields : TreeViewWithType<TextEditorDebugObjectWrap>
{
    private readonly ITreeViewRenderers _treeViewRenderers;

    public TreeViewFields(
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
            obj is not TreeViewFields treeViewFields)
        {
            return false;
        }

        return treeViewFields.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item?.GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewRenderers.TreeViewFieldsRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewFields),
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

            var fieldInfos = Item.DebugObjectItemType
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        
            foreach (var fieldInfo in fieldInfos)
            {
                var childValue = Item.DebugObjectItem is null
                    ? null
                    : fieldInfo.GetValue(Item.DebugObjectItem);

                var childType = fieldInfo.FieldType;
            
                var childNode = new TextEditorDebugObjectWrap(
                    childValue,
                    childType,
                    fieldInfo.Name,
                    fieldInfo.IsPublic);
            
                Children.Add(new TreeViewReflection(
                    childNode,
                    true,
                    false,
                    _treeViewRenderers));
            }

            if (Children.Count == 0)
            {
                Children.Add(new TreeViewText(
                    "No fields exist for this Type",
                    false,
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