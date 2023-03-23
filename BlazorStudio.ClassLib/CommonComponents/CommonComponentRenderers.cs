namespace BlazorStudio.ClassLib.CommonComponents;

public class CommonComponentRenderers : ICommonComponentRenderers
{
    public CommonComponentRenderers(
        Type? inputFileRenderer,
        Type? informativeNotificationRenderer,
        Type? errorNotificationRenderer,
        Type? fileFormRendererType,
        Type? deleteFileFormRendererType,
        Type? treeViewMissingRendererFallbackType,
        Type? treeViewNamespacePathRendererType,
        Type? treeViewExceptionRendererType,
        Type? treeViewAbsoluteFilePathRendererType,
        Type? treeViewGitFileRendererType,
        Type? nuGetPackageManagerRendererType,
        Type? gitDisplayRendererType,
        Type? removeCSharpProjectFromSolutionRendererType,
        Type? booleanPromptOrCancelRendererType)
    {
        // Validate
        {
            ValidateRenderer(
                inputFileRenderer, 
                typeof(IInputFileRendererType),
                t => InputFileRendererType = t);
                
            ValidateRenderer(
                informativeNotificationRenderer, 
                typeof(IInformativeNotificationRendererType),
                t => InformativeNotificationRendererType = t);
                
            ValidateRenderer(
                errorNotificationRenderer, 
                typeof(IErrorNotificationRendererType),
                t => ErrorNotificationRendererType = t);
                
            ValidateRenderer(
                fileFormRendererType, 
                typeof(IFileFormRendererType),
                t => FileFormRendererType = t);
            
            ValidateRenderer(
                deleteFileFormRendererType, 
                typeof(IDeleteFileFormRendererType),
                t => DeleteFileFormRendererType = t);
            
            ValidateRenderer(
                treeViewMissingRendererFallbackType, 
                typeof(ITreeViewMissingRendererFallbackType),
                t => TreeViewMissingRendererFallbackType = t);
            
            ValidateRenderer(
                treeViewNamespacePathRendererType, 
                typeof(ITreeViewNamespacePathRendererType),
                t => TreeViewNamespacePathRendererType = t);
            
            ValidateRenderer(
                treeViewExceptionRendererType, 
                typeof(ITreeViewExceptionRendererType),
                t => TreeViewExceptionRendererType = t);
            
            ValidateRenderer(
                treeViewAbsoluteFilePathRendererType, 
                typeof(ITreeViewAbsoluteFilePathRendererType),
                t => TreeViewAbsoluteFilePathRendererType = t);
            
            ValidateRenderer(
                treeViewGitFileRendererType, 
                typeof(ITreeViewGitFileRendererType),
                t => TreeViewGitFileRendererType = t);
            
            ValidateRenderer(
                nuGetPackageManagerRendererType, 
                typeof(INuGetPackageManagerRendererType),
                t => NuGetPackageManagerRendererType = t);
            
            ValidateRenderer(
                gitDisplayRendererType, 
                typeof(IGitDisplayRendererType),
                t => GitDisplayRendererType = t);
            
            ValidateRenderer(
                removeCSharpProjectFromSolutionRendererType, 
                typeof(IRemoveCSharpProjectFromSolutionRendererType),
                t => RemoveCSharpProjectFromSolutionRendererType = t);
            
            ValidateRenderer(
                booleanPromptOrCancelRendererType, 
                typeof(IBooleanPromptOrCancelRendererType),
                t => BooleanPromptOrCancelRendererType = t);
        }
        
        void ValidateRenderer(
            Type? implementationType,
            Type interfaceType,
            Action<Type?> setPropertyFunc)
        {
            // If "implementationType is null" then the UI Element is not in use and can be skipped.
            if (implementationType is null)
                return;
            
            if (implementationType.GetInterface(interfaceType.Name) is null)
            {
                throw new ApplicationException(
                    $"The {nameof(implementationType)} provided: {implementationType.Name}''" +
                    $" does not implement the required interface:" +
                    $" {nameof(interfaceType.Name)}");
            }

            setPropertyFunc.Invoke(implementationType);
        }
    }

    public Type? InputFileRendererType { get; private set; }
    public Type? InformativeNotificationRendererType { get; private set; }
    public Type? ErrorNotificationRendererType { get; private set; }
    public Type? FileFormRendererType { get; private set; }
    public Type? DeleteFileFormRendererType { get; private set; }
    public Type? TreeViewMissingRendererFallbackType { get; private set; }
    public Type? TreeViewNamespacePathRendererType { get; private set; }
    public Type? TreeViewExceptionRendererType { get; private set; }
    public Type? TreeViewAbsoluteFilePathRendererType { get; private set; }
    public Type? TreeViewGitFileRendererType { get; private set; }
    public Type? NuGetPackageManagerRendererType { get; private set; }
    public Type? GitDisplayRendererType { get; private set; }
    public Type? RemoveCSharpProjectFromSolutionRendererType { get; private set; }
    public Type? BooleanPromptOrCancelRendererType { get; private set; }
}