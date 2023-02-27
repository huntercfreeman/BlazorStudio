using System.Reflection;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;

namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

public class TreeViewProperties : TreeViewWithType<TextEditorDebugObjectWrap>
{
    private readonly ITreeViewRenderers _treeViewRenderers;

    public TreeViewProperties(
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
            obj is not TreeViewProperties treeViewProperties)
        {
            return false;
        }

        return treeViewProperties.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item?.GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewRenderers.TreeViewPropertiesRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewProperties),
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

            var propertyInfos = Item.DebugObjectItemType
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            foreach (var propertyInfo in propertyInfos)
            {
                try
                {
                    var childValue = Item.DebugObjectItem is null
                        ? null
                        : propertyInfo.GetValue(Item.DebugObjectItem);

                    var childType = propertyInfo.PropertyType;
                
                    // https://stackoverflow.com/questions/3762456/how-to-check-if-property-setter-is-public
                    //
                    // The getter exists and is public.
                    var hasPublicGetter = propertyInfo.CanRead &&
                                          (propertyInfo.GetGetMethod( /*nonPublic*/ true)?.IsPublic ?? false);
            
                    var childNode = new TextEditorDebugObjectWrap(
                        childValue,
                        childType,
                        propertyInfo.Name,
                        hasPublicGetter);
            
                    Children.Add(new TreeViewReflection(
                        childNode,
                        true,
                        false,
                        _treeViewRenderers));
                }
                catch (System.Reflection.TargetParameterCountException)
                {
                    // Types: { 'string', 'ImmutableArray<TItem>' } at the minimum
                    // at throwing System.Reflection.TargetParameterCountException
                    // and it appears to be due to a propertyInfo for the generic type argument?
                    //
                    // Given the use case for code I am okay with continuing when this exception
                    // happens as it seems unrelated to the point of this class.
                }
            }

            if (Children.Count == 0)
            {
                Children.Add(new TreeViewText(
                    "No properties exist for this Type",
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