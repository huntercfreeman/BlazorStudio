using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.ClassLib.Store.KeyDownEventCase;

public record KeyDownEventAction(PlainTextEditorKey FocusedPlainTextEditorKey, 
    KeyDownEventRecord KeyDownEventRecord);
