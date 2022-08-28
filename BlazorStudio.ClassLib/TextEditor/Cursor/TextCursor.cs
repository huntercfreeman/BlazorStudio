using ExperimentalTextEditor.ClassLib.TextEditor.Enums;
using ExperimentalTextEditor.ClassLib.TextEditor.IndexWrappers;

namespace ExperimentalTextEditor.ClassLib.TextEditor;

public class TextCursor
{
    public RowIndex RowIndex { get; set; }
    public ColumnIndex ColumnIndex { get; set; }
    public TextCursorKind TextCursorKind { get; set; }
}