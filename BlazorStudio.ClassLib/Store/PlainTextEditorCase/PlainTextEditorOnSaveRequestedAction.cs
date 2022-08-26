namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorOnSaveRequestedAction(
    PlainTextEditorKey PlainTextEditorKey,
    CancellationToken CancellationToken);