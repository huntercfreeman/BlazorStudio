namespace BlazorStudio.ClassLib.CommonComponents;

public interface ICommonComponentRenderers
{
    public Type InputFileRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ErrorNotificationRendererType { get; }
    public Type FileFormRendererType { get; set; }
    public Type DeleteFileFormRendererType { get; set; }
}