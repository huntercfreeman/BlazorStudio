using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public record SetSolutionAction(MSBuildWorkspace? SolutionWorkspace);