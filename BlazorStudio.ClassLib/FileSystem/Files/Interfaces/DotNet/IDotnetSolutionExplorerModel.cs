using BlazorStudio.ClassLib.FileSystem.Files.Interfaces.Files.DotNet;

namespace BlazorStudio.ClassLib.FileSystem.Files.Interfaces.DotNet;

public interface IDotnetSolutionExplorerModel
{
    public IDotNetSolutionFileModel? SelectedDotnetSolution { get; set; }
}