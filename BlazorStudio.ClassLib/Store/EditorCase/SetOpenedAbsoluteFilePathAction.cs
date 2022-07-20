using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.EditorCase;

public record SetOpenedAbsoluteFilePathAction(IAbsoluteFilePath? AbsoluteFilePath);