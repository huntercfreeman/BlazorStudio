namespace BlazorStudio.ClassLib.DotNet;

public interface IDotNetProject
{
    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public Guid ProjectIdGuid { get; }
}