namespace BlazorStudio.ClassLib;

public interface ICommonComponentRenderers
{
    public Type InputFileRendererType { get; }
    public Type InformativeNotificationRenderer { get; }
    public Type ErrorNotificationRenderer { get; }
}