using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet.CSharp;

namespace BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet;

public interface IDotNetSolutionFileModel : IFileModel
{
    /// <summary>
    /// Null CSharpProjects implies a version of LoadCSharpProjects
    /// has yet to be called.
    /// </summary>
    public List<ICSharpProjectFileModel> CSharpProjects { get; set; }
    public bool CSharpProjectsInitiallyLoaded { get; }

    public void LoadCSharpProjects();
    public Task LoadCSharpProjectsAsync();
}