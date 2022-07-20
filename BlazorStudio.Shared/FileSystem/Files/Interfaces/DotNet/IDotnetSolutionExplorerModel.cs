using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet;

namespace BlazorStudio.Shared.FileSystem.Files.Interfaces.DotNet;

public interface IDotnetSolutionExplorerModel
{
    public IDotNetSolutionFileModel? SelectedDotnetSolution { get; set; }
}