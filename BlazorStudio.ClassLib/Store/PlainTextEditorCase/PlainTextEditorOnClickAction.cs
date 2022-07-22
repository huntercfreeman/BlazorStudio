namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorOnClickAction(PlainTextEditorKey FocusedPlainTextEditorKey,
    int RowIndex,
    int TokenIndex,
    int? CharacterIndex);