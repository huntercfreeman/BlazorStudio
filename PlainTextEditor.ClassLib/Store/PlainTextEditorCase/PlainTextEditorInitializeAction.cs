using BlazorStudio.Shared.FileSystem.Interfaces;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorInitializeAction(PlainTextEditorKey FocusedPlainTextEditorKey,
    IAbsoluteFilePath AbsoluteFilePath);