using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor.Cursor;

public class TextCursor
{
    /// <summary>
    /// When changing both <see cref="RowIndex"/> and <see cref="ColumnIndex"/>
    /// this property being a Tuple helps change coordinates as a 'transaction'.
    /// </summary>
    public (RowIndex RowIndex, ColumnIndex ColumnIndex) IndexCoordinates { get; set; }
        = (new(0), new(0));
    public TextCursorKind TextCursorKind { get; set; } = TextCursorKind.Beam;
}