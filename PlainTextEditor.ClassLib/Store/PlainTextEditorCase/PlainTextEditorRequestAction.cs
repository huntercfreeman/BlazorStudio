using BlazorStudio.Shared.FileSystem.Classes;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public record PlainTextEditorRequestAction(PlainTextEditorKey FocusedPlainTextEditorKey, 
    FileCoordinateGridRequest FileCoordinateGridRequest);