namespace BlazorStudio.ClassLib.CommonComponents;

public class CommonComponentRenderers : ICommonComponentRenderers
{
    public CommonComponentRenderers(
        Type inputFileRenderer,
        Type informativeNotificationRenderer,
        Type errorNotificationRenderer,
        Type fileFormRendererType,
        Type deleteFileFormRendererType,
        Type treeViewNamespacePathRendererType,
        Type treeViewExceptionRendererType,
        Type treeViewAbsoluteFilePathRendererType)
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
            
            ValidateRenderer(
                deleteFileFormRendererType, 
                typeof(IDeleteFileFormRendererType));
            
            ValidateRenderer(
                treeViewNamespacePathRendererType, 
                typeof(ITreeViewNamespacePathRendererType));
            
            ValidateRenderer(
                treeViewExceptionRendererType, 
                typeof(ITreeViewExceptionRendererType));
            
            ValidateRenderer(
                treeViewAbsoluteFilePathRendererType, 
                typeof(ITreeViewAbsoluteFilePathRendererType));
        }

        // Assign
        {
            InputFileRendererType = inputFileRenderer;
            InformativeNotificationRendererType = informativeNotificationRenderer;
            ErrorNotificationRendererType = errorNotificationRenderer;
            FileFormRendererType = fileFormRendererType;
            DeleteFileFormRendererType = deleteFileFormRendererType;
            TreeViewNamespacePathRendererType = treeViewNamespacePathRendererType;
            TreeViewExceptionRendererType = treeViewExceptionRendererType;
            TreeViewAbsoluteFilePathRendererType = treeViewAbsoluteFilePathRendererType;
        }
    }

    public Type InputFileRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ErrorNotificationRendererType { get; }
    public Type FileFormRendererType { get; }
    public Type DeleteFileFormRendererType { get; }
    public Type TreeViewNamespacePathRendererType { get; }
    public Type TreeViewExceptionRendererType { get; }
    public Type TreeViewAbsoluteFilePathRendererType { get; set; }
}