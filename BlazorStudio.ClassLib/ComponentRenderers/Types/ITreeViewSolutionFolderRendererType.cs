using BlazorStudio.ClassLib.DotNet;

namespace BlazorStudio.ClassLib.ComponentRenderers.Types;

public interface ITreeViewSolutionFolderRendererType
{
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; }
}