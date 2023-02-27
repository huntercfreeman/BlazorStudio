namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

public interface ITreeViewRenderers
{
    public Type TreeViewTextRenderer { get; }
    public Type TreeViewReflectionRenderer { get; }
    public Type TreeViewPropertiesRenderer { get; }
    public Type TreeViewInterfaceImplementationRenderer { get; }
    public Type TreeViewFieldsRenderer { get; }
    public Type TreeViewExceptionRenderer { get; }
    public Type TreeViewEnumerableRenderer { get; }
}