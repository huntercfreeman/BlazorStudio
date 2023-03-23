using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.DotNet;

public class DotNetSolution
{
    public DotNetSolution(
        string displayName, 
        ImmutableList<IDotNetProject> dotNetProjects)
    {
        DisplayName = displayName;
        DotNetProjects = dotNetProjects;
    }

    public string DisplayName { get; }
    public ImmutableList<IDotNetProject> DotNetProjects { get; }
}