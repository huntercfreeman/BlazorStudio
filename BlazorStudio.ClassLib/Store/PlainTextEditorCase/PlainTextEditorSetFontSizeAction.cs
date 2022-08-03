namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorSetFontSizeAction(PlainTextEditorKey PlainTextEditorKey,
    int FontSize);