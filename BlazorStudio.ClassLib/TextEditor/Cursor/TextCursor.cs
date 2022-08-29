using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor.Cursor;

public class TextCursor
{
    public RowIndex RowIndex { get; set; } = new(0);
    public ColumnIndex ColumnIndex { get; set; } = new(0);
    public TextCursorKind TextCursorKind { get; set; } = TextCursorKind.Beam;
}