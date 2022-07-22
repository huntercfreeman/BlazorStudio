using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.EditorCase;

public record SetOpenedAbsoluteFilePathAction(IAbsoluteFilePath? AbsoluteFilePath);