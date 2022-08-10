using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystemApi;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public record SetSolutionFileDocumentMapAction(ImmutableDictionary<AbsoluteFilePathStringValue, Document> FileDocumentMap);