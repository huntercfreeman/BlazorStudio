using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.RoslynHelpers;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public record SetSolutionFileDocumentMapAction(ImmutableDictionary<AbsoluteFilePathStringValue, IndexedDocument> FileDocumentMap);