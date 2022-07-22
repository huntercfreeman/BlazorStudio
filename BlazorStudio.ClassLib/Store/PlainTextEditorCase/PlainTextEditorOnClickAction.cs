namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorOnClickAction(PlainTextEditorKey PlainTextEditorKey,
    int RowIndex,
    int TokenIndex,
    int? CharacterIndex);