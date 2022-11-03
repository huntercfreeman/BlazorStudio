namespace BlazorStudio.ClassLib;

public class CommonComponentRenderers : ICommonComponentRenderers
{
    public CommonComponentRenderers(
        Type inputFileRenderer,
        Type informativeNotificationRenderer,
        Type errorNotificationRenderer)
    {
        InputFileRendererType = inputFileRenderer;
        InformativeNotificationRenderer = informativeNotificationRenderer;
        ErrorNotificationRenderer = errorNotificationRenderer;
    }

    public Type InputFileRendererType { get; }
    public Type InformativeNotificationRenderer { get; }
    public Type ErrorNotificationRenderer { get; }
}