namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

public class TreeViewRenderers : ITreeViewRenderers
{
    public TreeViewRenderers(
        Type treeViewTextRenderer,
        Type treeViewReflectionRenderer,
        Type treeViewPropertiesRenderer,
        Type treeViewInterfaceImplementationRenderer,
        Type treeViewFieldsRenderer,
        Type treeViewExceptionRenderer,
        Type treeViewEnumerableRenderer)
    {
        TreeViewTextRenderer = treeViewTextRenderer;
        TreeViewReflectionRenderer = treeViewReflectionRenderer;
        TreeViewPropertiesRenderer = treeViewPropertiesRenderer;
        TreeViewInterfaceImplementationRenderer = treeViewInterfaceImplementationRenderer;
        TreeViewFieldsRenderer = treeViewFieldsRenderer;
        TreeViewExceptionRenderer = treeViewExceptionRenderer;
        TreeViewEnumerableRenderer = treeViewEnumerableRenderer;
    }

    public Type TreeViewTextRenderer { get; }
    public Type TreeViewReflectionRenderer { get; }
    public Type TreeViewPropertiesRenderer { get; }
    public Type TreeViewInterfaceImplementationRenderer { get; }
    public Type TreeViewFieldsRenderer { get; }
    public Type TreeViewExceptionRenderer { get; }
    public Type TreeViewEnumerableRenderer { get; }
}