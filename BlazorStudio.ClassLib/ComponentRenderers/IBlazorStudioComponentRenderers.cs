using BlazorCommon.RazorLib.ComponentRenderers;

namespace BlazorStudio.ClassLib.ComponentRenderers;

public interface IBlazorStudioComponentRenderers
{
    public IBlazorCommonComponentRenderers? BlazorCommonComponentRenderers { get; }
    public Type? FileFormRendererType { get; }
    public Type? DeleteFileFormRendererType { get; }
    public Type? TreeViewNamespacePathRendererType { get; }
    public Type? TreeViewAbsoluteFilePathRendererType { get; }
    public Type? TreeViewGitFileRendererType { get; }
    public Type? NuGetPackageManagerRendererType { get; }
    public Type? GitDisplayRendererType { get; }
    public Type? RemoveCSharpProjectFromSolutionRendererType { get; }
    public Type? InputFileRendererType { get; }
}