using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.WorkspaceCase;

public record SetWorkspaceAction(IAbsoluteFilePath? WorkspaceAbsoluteFilePath);