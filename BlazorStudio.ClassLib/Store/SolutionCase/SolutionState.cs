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

namespace BlazorStudio.ClassLib.Store.SolutionCase;

[FeatureState]
public record SolutionState(MSBuildWorkspace? SolutionWorkspace, 
    VisualStudioInstance? VisualStudioInstance,
    IAbsoluteFilePath? MsBuildAbsoluteFilePath,
    ImmutableDictionary<AbsoluteFilePathStringValue, IndexedDocument> FileDocumentMap)
{
    private SolutionState() : this(default(MSBuildWorkspace), 
        default(VisualStudioInstance),
        default(IAbsoluteFilePath),
        ImmutableDictionary<AbsoluteFilePathStringValue, IndexedDocument>.Empty)
    {

    }
}