using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.RoslynWorkspaceState;

public record SetRoslynWorkspaceStateAction(MSBuildWorkspace? MsBuildWorkspace,
    VisualStudioInstance? VisualStudioInstance,
    IAbsoluteFilePath? MsBuildAbsoluteFilePath);