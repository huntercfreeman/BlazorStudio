using System.Collections;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;

namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

public class TreeViewEnumerable : TreeViewWithType<TextEditorDebugObjectWrap>
{
    private readonly ITreeViewRenderers _treeViewRenderers;

    public TreeViewEnumerable(
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
            obj is not TreeViewEnumerable treeViewEnumerable)
        {
            return false;
        }

        return treeViewEnumerable.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item?.GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewRenderers.TreeViewEnumerableRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewEnumerable),
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
            
            if (Item.DebugObjectItem is IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();

                var genericArgument = GetGenericArgument(Item.DebugObjectItem.GetType());
                
                while (enumerator.MoveNext())
                {
                    var entry = enumerator.Current;
                    
                    var childNode = new TextEditorDebugObjectWrap(
                        entry,
                        genericArgument,
                        genericArgument.Name,
                        Item.IsPubliclyReadable);
            
                    Children.Add(new TreeViewReflection(
                        childNode,
                        true,
                        false,
                        _treeViewRenderers));
                }
            }
            else
            {
                throw new ApplicationException(
                    $"Unexpected failed cast to the Type {nameof(IEnumerable)}." +
                    $" {nameof(TreeViewEnumerable)} are to have a {nameof(Item.DebugObjectItem)} which is castable as {nameof(IEnumerable)}");
            }

            if (Children.Count == 0)
            {
                Children.Add(new TreeViewText(
                    "Enumeration returned no results.",
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

    // https://stackoverflow.com/questions/906499/getting-type-t-from-ienumerablet
    private static Type GetGenericArgument(Type type)
    {
        // Type is Array
        // short-circuit if you expect lots of arrays 
        if (type.IsArray)
            return type.GetElementType()!;

        // type is IEnumerable<T>;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IEnumerable<>))
            return type.GetGenericArguments()[0];

        // type implements/extends IEnumerable<T>;
        var enumType = type.GetInterfaces()
            .Where(t => t.IsGenericType && 
                        t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
        return enumType ?? type;
    }
}