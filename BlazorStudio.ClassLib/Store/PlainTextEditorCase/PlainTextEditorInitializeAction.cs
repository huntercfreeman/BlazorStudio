using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorInitializeAction(PlainTextEditorKey FocusedPlainTextEditorKey,
    IAbsoluteFilePath AbsoluteFilePath);