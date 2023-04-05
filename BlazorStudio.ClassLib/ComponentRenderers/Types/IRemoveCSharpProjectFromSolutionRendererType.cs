using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.ComponentRenderers;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
    public IAbsoluteFilePath AbsoluteFilePath { get; set; }
    public Action<IAbsoluteFilePath> OnAfterSubmitAction { get; set; }
}