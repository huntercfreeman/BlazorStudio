using BlazorStudio.ClassLib.FileSystem.Classes;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record MemoryMappedFileExactReadRequestAction(PlainTextEditorKey PlainTextEditorKey,
    FileCoordinateGridRequest FileCoordinateGridRequest);