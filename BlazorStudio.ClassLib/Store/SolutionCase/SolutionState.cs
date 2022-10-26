using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.RoslynHelpers;
using Fluxor;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

[FeatureState]
public record SolutionState(Solution? Solution,
    ImmutableDictionary<ProjectId, IndexedProject> ProjectIdToProjectMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, IndexedDocument> FileAbsoluteFilePathToDocumentMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, IndexedAdditionalDocument>
        FileAbsoluteFilePathToAdditionalDocumentMap)
{
    private SolutionState() : this(default,
        ImmutableDictionary<ProjectId, IndexedProject>.Empty,
        ImmutableDictionary<AbsoluteFilePathStringValue, IndexedDocument>.Empty,
        ImmutableDictionary<AbsoluteFilePathStringValue, IndexedAdditionalDocument>.Empty)
    {
    }
}