using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
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

public record SetSolutionStateAction(Solution? Solution,
    ImmutableDictionary<ProjectId, Project> ProjectIdToProjectMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, IndexedDocument> FileAbsoluteFilePathToDocumentMap);