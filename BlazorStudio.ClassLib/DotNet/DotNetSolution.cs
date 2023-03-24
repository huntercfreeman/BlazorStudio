using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.DotNet;

public record DotNetSolution(
    string DisplayName,
    ImmutableList<IDotNetProject> DotNetProjects);