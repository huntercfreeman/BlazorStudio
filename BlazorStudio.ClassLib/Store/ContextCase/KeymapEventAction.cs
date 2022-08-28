using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.ContextCase;

public record KeymapEventAction(
    KeyDownEventRecord KeyDownEventRecord, 
    object Parameters,
    CancellationToken CancellationToken);
