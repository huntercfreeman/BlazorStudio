using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Files.Interfaces.Files.DotNet;

namespace BlazorStudio.ClassLib.FileSystem.Files.Interfaces.DotNet;

public interface IDotNetSolutionsProviderModel
{
    public ImmutableArray<IDotNetSolutionFileModel> DotnetSolutions { get; }
    public void LoadDotnetSolutions();
    public Task LoadDotnetSolutionsAsync();
}