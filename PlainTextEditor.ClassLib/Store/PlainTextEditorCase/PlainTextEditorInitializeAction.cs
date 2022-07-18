namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorInitializeAction(PlainTextEditorKey PlainTextEditorKey,
    string AbsoluteFilePathString);