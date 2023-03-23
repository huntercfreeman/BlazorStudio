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
        InputFileRendererType = inputFileRenderer;
        InformativeNotificationRendererType = informativeNotificationRenderer;
        ErrorNotificationRendererType = errorNotificationRenderer;
        FileFormRendererType = fileFormRendererType;
        DeleteFileFormRendererType = deleteFileFormRendererType;
        TreeViewMissingRendererFallbackType = treeViewMissingRendererFallbackType;
        TreeViewNamespacePathRendererType = treeViewNamespacePathRendererType;
        TreeViewExceptionRendererType = treeViewExceptionRendererType;
        TreeViewAbsoluteFilePathRendererType = treeViewAbsoluteFilePathRendererType;
        TreeViewGitFileRendererType = treeViewGitFileRendererType;
        NuGetPackageManagerRendererType = nuGetPackageManagerRendererType;
        GitDisplayRendererType = gitDisplayRendererType;
        RemoveCSharpProjectFromSolutionRendererType = removeCSharpProjectFromSolutionRendererType;
        BooleanPromptOrCancelRendererType = booleanPromptOrCancelRendererType;
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