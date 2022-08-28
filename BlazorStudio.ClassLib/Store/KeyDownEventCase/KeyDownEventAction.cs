using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.KeyDownEventCase;

public record KeyDownEventAction(KeyDownEventRecord KeyDownEventRecord,
    CancellationToken CancellationToken);
