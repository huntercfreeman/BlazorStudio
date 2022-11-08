namespace BlazorStudio.ClassLib.CommonComponents;

public interface ICommonComponentRenderers
{
    public Type InputFileRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ErrorNotificationRendererType { get; }
    public Type FileFormRendererType { get; }
    public Type DeleteFileFormRendererType { get; }
    public Type TreeViewNamespacePathRendererType { get; }
    public Type TreeViewExceptionRendererType { get; }
}