namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorOnClickAction(PlainTextEditorKey PlainTextEditorKey,
    int RowIndex,
    int TokenIndex,
    int? CharacterIndex,
    bool ShiftKey,
    CancellationToken CancellationToken);