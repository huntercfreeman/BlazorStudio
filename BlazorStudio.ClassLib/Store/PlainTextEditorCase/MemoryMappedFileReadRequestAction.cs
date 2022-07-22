using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record MemoryMappedFileReadRequestAction(PlainTextEditorKey PlainTextEditorKey, 
    VirtualizeCoordinateSystemRequest VirtualizeCoordinateSystemRequest);