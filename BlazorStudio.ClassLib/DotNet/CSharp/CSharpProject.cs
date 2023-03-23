namespace BlazorStudio.ClassLib.DotNet.CSharp;

public class CSharpProject : IDotNetProject
{
    public CSharpProject(
        string displayName,
        Guid projectTypeGuid,
        Guid projectIdGuid)
    {
        DisplayName = displayName;
        ProjectTypeGuid = projectTypeGuid;
        ProjectIdGuid = projectIdGuid;
    }
    
    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public Guid ProjectIdGuid { get; }
}