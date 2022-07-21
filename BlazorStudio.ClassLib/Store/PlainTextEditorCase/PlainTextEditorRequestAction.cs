using BlazorStudio.ClassLib.FileSystem.Classes;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorRequestAction(PlainTextEditorKey FocusedPlainTextEditorKey, 
    FileCoordinateGridRequest FileCoordinateGridRequest);