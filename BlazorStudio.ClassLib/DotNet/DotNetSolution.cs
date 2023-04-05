using System.Collections.Immutable;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.DotNet;

public record DotNetSolution(
    NamespacePath NamespacePath,
    ImmutableList<IDotNetProject> DotNetProjects);