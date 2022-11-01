namespace BlazorStudio.ClassLib;

public class CommonRendererTypes : ICommonComponentRenderers
{
    public CommonRendererTypes(
        Type inputFileRenderer)
    {
        InputFileRendererType = inputFileRenderer;
    }

    public Type InputFileRendererType { get; }
}