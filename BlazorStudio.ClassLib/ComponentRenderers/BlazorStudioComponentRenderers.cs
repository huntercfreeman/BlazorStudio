using BlazorCommon.RazorLib.ComponentRenderers;

namespace BlazorStudio.ClassLib.ComponentRenderers;

public class BlazorStudioComponentRenderers : IBlazorStudioComponentRenderers
{
    public BlazorStudioComponentRenderers(
        IBlazorCommonComponentRenderers? blazorCommonComponentRenderers,
        Type? fileFormRendererType,
        Type? deleteFileFormRendererType,
        Type? treeViewNamespacePathRendererType,
        Type? treeViewAbsoluteFilePathRendererType,
        Type? treeViewGitFileRendererType,
        Type? nuGetPackageManagerRendererType,
        Type? gitDisplayRendererType,
        Type? removeCSharpProjectFromSolutionRendererType,
        Type? inputFileRendererType,
        Type? treeViewCSharpProjectDependenciesRendererType,
        Type? treeViewCSharpProjectNugetPackageReferencesRendererType,
        Type? treeViewCSharpProjectToProjectReferencesRendererType,
        Type? treeViewLightWeightNugetPackageRecordRendererType)
    {
        BlazorCommonComponentRenderers = blazorCommonComponentRenderers;
        FileFormRendererType = fileFormRendererType;
        DeleteFileFormRendererType = deleteFileFormRendererType;
        TreeViewNamespacePathRendererType = treeViewNamespacePathRendererType;
        TreeViewAbsoluteFilePathRendererType = treeViewAbsoluteFilePathRendererType;
        TreeViewGitFileRendererType = treeViewGitFileRendererType;
        NuGetPackageManagerRendererType = nuGetPackageManagerRendererType;
        GitDisplayRendererType = gitDisplayRendererType;
        RemoveCSharpProjectFromSolutionRendererType = removeCSharpProjectFromSolutionRendererType;
        InputFileRendererType = inputFileRendererType;
        TreeViewCSharpProjectDependenciesRendererType = treeViewCSharpProjectDependenciesRendererType;
        TreeViewCSharpProjectNugetPackageReferencesRendererType = treeViewCSharpProjectNugetPackageReferencesRendererType;
        TreeViewCSharpProjectToProjectReferencesRendererType = treeViewCSharpProjectToProjectReferencesRendererType;
        TreeViewLightWeightNugetPackageRecordRendererType = treeViewLightWeightNugetPackageRecordRendererType;
    }

    public IBlazorCommonComponentRenderers? BlazorCommonComponentRenderers { get; }
    public Type? FileFormRendererType { get; }
    public Type? DeleteFileFormRendererType { get; }
    public Type? TreeViewNamespacePathRendererType { get; }
    public Type? TreeViewCSharpProjectDependenciesRendererType { get; }
    public Type? TreeViewCSharpProjectNugetPackageReferencesRendererType { get; }
    public Type? TreeViewCSharpProjectToProjectReferencesRendererType { get; }
    public Type? TreeViewLightWeightNugetPackageRecordRendererType { get; }
    public Type? TreeViewAbsoluteFilePathRendererType { get; }
    public Type? TreeViewGitFileRendererType { get; }
    public Type? NuGetPackageManagerRendererType { get; }
    public Type? GitDisplayRendererType { get; }
    public Type? RemoveCSharpProjectFromSolutionRendererType { get; }
    public Type? InputFileRendererType { get; }
}