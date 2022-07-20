using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.WorkspaceCase;

public record SetWorkspaceAction(IAbsoluteFilePath? WorkspaceAbsoluteFilePath);