namespace BlazorStudio.ClassLib;

public class CommonRendererTypes : ICommonComponentRenderers
{
    public CommonRendererTypes(
        Type inputFileRenderer,
        Type informativeNotificationRenderer,
        Type errorNotificationRenderer)
    {
        InputFileRendererType = inputFileRenderer;
    }

    public Type InputFileRendererType { get; }
}