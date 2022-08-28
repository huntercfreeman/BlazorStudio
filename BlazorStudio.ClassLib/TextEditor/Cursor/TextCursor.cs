using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor.Cursor;

public class TextCursor
{
    public RowIndex RowIndex { get; set; }
    public ColumnIndex ColumnIndex { get; set; }
    public TextCursorKind TextCursorKind { get; set; }
}