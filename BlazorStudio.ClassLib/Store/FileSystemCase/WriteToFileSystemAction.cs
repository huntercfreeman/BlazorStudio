using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.FileSystemCase;

public record WriteToFileSystemAction(IAbsoluteFilePath AbsoluteFilePath, string Content);