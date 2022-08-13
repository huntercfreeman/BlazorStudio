using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record ConstructTokenizedPlainTextEditorRecordAction(PlainTextEditorKey PlainTextEditorKey,
    IAbsoluteFilePath AbsoluteFilePath,
    IFileSystemProvider FileSystemProvider,
    CancellationToken CancellationToken);