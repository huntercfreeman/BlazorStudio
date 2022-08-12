namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorOnMouseOverCharacterAction(PlainTextEditorKey PlainTextEditorKey,
    int RowIndex,
    int TokenIndex,
    int? CharacterIndex,
    bool ShiftKey,
    CancellationToken CancellationToken);