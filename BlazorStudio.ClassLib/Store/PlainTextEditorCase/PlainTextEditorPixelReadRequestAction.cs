using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorPixelReadRequestAction(PlainTextEditorKey PlainTextEditorKey, 
    VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage);