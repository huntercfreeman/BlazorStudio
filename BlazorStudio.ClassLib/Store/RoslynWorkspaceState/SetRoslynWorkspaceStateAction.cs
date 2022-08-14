using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.RoslynWorkspaceState;

public record SetRoslynWorkspaceStateAction(MSBuildWorkspace? MSBuildWorkspace, 
    VisualStudioInstance? VisualStudioInstance,
    IAbsoluteFilePath? MsBuildAbsoluteFilePath);