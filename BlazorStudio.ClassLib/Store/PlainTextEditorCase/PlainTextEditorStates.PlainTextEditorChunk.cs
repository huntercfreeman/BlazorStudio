using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorChunk(PlainTextEditorRowKey Key,
        FileCoordinateGridRequest FileCoordinateGridRequest,
        List<string> ContentOfRows)
    {
    }
}
