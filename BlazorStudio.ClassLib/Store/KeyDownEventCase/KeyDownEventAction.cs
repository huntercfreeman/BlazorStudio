using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.KeyDownEventCase;

public record KeyDownEventAction(PlainTextEditorKey PlainTextEditorKey, 
    KeyDownEventRecord KeyDownEventRecord,
    CancellationToken CancellationToken);
