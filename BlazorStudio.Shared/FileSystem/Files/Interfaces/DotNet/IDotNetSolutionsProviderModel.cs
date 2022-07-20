using System.Collections.Immutable;
using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet;

namespace BlazorStudio.Shared.FileSystem.Files.Interfaces.DotNet;

public interface IDotNetSolutionsProviderModel
{
    public ImmutableArray<IDotNetSolutionFileModel> DotnetSolutions { get; }
    public void LoadDotnetSolutions();
    public Task LoadDotnetSolutionsAsync();
}