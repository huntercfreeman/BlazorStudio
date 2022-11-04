namespace BlazorStudio.ClassLib.CommonComponents;

public class CommonComponentRenderers : ICommonComponentRenderers
{
    public CommonComponentRenderers(
        Type inputFileRenderer,
        Type informativeNotificationRenderer,
        Type errorNotificationRenderer,
        Type fileFormRendererType)
    {
        void ValidateRenderer(Type implementationType, Type interfaceType)
        {
            if (inputFileRenderer.GetInterface(nameof(IInputFileRendererType)) is null)
            {
                throw new ApplicationException(
                    $"The {nameof(implementationType)} provided: {implementationType.Name}''" +
                    $" does not implement the required interface:" +
                    $" {nameof(interfaceType.Name)}");
            }
        }
        
        // Validate
        {
            ValidateRenderer(
                inputFileRenderer, 
                typeof(IInputFileRendererType));
                
            ValidateRenderer(
                informativeNotificationRenderer, 
                typeof(IInformativeNotificationRendererType));
                
            ValidateRenderer(
                errorNotificationRenderer, 
                typeof(IErrorNotificationRendererType));
                
            ValidateRenderer(
                fileFormRendererType, 
                typeof(IFileFormRendererType));
        }

        // Assign
        {
            InputFileRendererType = inputFileRenderer;
            InformativeNotificationRendererType = informativeNotificationRenderer;
            ErrorNotificationRendererType = errorNotificationRenderer;
            FileFormRendererType = fileFormRendererType;    
        }
    }

    public Type InputFileRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ErrorNotificationRendererType { get; }
    public Type FileFormRendererType { get; set; }
}