namespace BlazorStudio.ClassLib.DotNet.DotNetSolutionGlobalSectionTypes;

public record NestedProjectEntry(
    Guid ChildProjectIdGuid,
    Guid SolutionFolderIdGuid);