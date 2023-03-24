namespace BlazorStudio.ClassLib.DotNet.CSharp;

public class CSharpProject : IDotNetProject
{
    public CSharpProject(
        string displayName,
        Guid projectTypeGuid,
        string relativePathFromSolutionFileString,
        Guid projectIdGuid)
    {
        DisplayName = displayName;
        ProjectTypeGuid = projectTypeGuid;
        RelativePathFromSolutionFileString = relativePathFromSolutionFileString;
        ProjectIdGuid = projectIdGuid;
    }
    
    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public string RelativePathFromSolutionFileString { get; }
    public Guid ProjectIdGuid { get; }
}