using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record ConstructMemoryMappedFilePlainTextEditorRecordAction(PlainTextEditorKey FocusedPlainTextEditorKey,
    IAbsoluteFilePath AbsoluteFilePath);