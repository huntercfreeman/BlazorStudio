using Fluxor;
using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.RoslynHelpers;
using Fluxor;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.RoslynWorkspaceState;

[FeatureState]
public record RoslynWorkspaceState(MSBuildWorkspace? MSBuildWorkspace,
    VisualStudioInstance? VisualStudioInstance,
    IAbsoluteFilePath? MsBuildAbsoluteFilePath)
{
    public RoslynWorkspaceState() : this(default(MSBuildWorkspace),
        default(VisualStudioInstance),
        default(IAbsoluteFilePath))
    {
        
    }
}