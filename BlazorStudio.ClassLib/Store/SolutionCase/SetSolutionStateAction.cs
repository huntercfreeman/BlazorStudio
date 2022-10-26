using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.RoslynHelpers;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public record SetSolutionStateAction(Solution? Solution,
    ImmutableDictionary<ProjectId, IndexedProject> ProjectIdToProjectMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, IndexedDocument> FileAbsoluteFilePathToDocumentMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, IndexedAdditionalDocument>
        FileAbsoluteFilePathToAdditionalDocumentMap);