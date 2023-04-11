using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.DotNet.DotNetSolutionGlobalSectionTypes;

public record GlobalSectionNestedProjects(
    ImmutableArray<NestedProjectEntry> NestedProjectEntries);