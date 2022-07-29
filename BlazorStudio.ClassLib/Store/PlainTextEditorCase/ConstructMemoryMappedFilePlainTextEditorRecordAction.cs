using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record ConstructMemoryMappedFilePlainTextEditorRecordAction(PlainTextEditorKey PlainTextEditorKey,
    IAbsoluteFilePath AbsoluteFilePath,
    IFileSystemProvider FileSystemProvider);