using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.DotNet;

public interface IDotNetProject
{
    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public string RelativePathFromSolutionFileString { get; }
    public Guid ProjectIdGuid { get; }
    public IAbsoluteFilePath AbsoluteFilePath { get; }
    public DotNetProjectKind DotNetProjectKind { get; }

    public void SetAbsoluteFilePath(IAbsoluteFilePath absoluteFilePath);
}