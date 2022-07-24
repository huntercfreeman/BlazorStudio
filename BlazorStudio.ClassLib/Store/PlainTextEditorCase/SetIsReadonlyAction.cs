namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record SetIsReadonlyAction(PlainTextEditorKey PlainTextEditorKey,
    bool IsReadonly);