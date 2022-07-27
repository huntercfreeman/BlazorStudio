using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record MemoryMappedFilePixelReadRequestAction(PlainTextEditorKey PlainTextEditorKey, 
    VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage);