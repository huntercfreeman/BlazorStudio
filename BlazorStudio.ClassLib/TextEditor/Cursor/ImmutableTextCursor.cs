using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;

namespace BlazorStudio.ClassLib.TextEditor.Cursor;

public record ImmutableTextCursor
{
    public ImmutableTextCursor(TextCursor textCursor)
    {
        IndexCoordinates = textCursor.IndexCoordinates;
        PreferredColumnIndex = textCursor.PreferredColumnIndex;
        TextCursorKind = textCursor.TextCursorKind;
    }
    
    /// <summary>
    /// When changing both <see cref="RowIndex"/> and <see cref="ColumnIndex"/>
    /// this property being a Tuple helps change coordinates as a 'transaction'.
    /// </summary>
    public (RowIndex RowIndex, ColumnIndex ColumnIndex) IndexCoordinates { get; init; }
        = (new(0), new(0));
    /// <summary>
    /// Store <see cref="ColumnIndex"/> after ArrowLeft or other movements that influence this.
    /// <br/><br/>
    /// When moving (ArrowUp or ArrowDown) one might come across a row that has a column length
    /// not long enough and <see cref="IndexCoordinates"/> will lower the <see cref="ColumnIndex"/>
    /// to an index within range.
    /// <br/><br/>
    /// If one then proceeds to find a row that WOULD have had a column length long enough.
    /// Then the <see cref="ColumnIndex"/> is restored using the stored <see cref="PreferredColumnIndex"/>.
    /// </summary>
    public ColumnIndex PreferredColumnIndex { get; init; } = new(0);
    public TextCursorKind TextCursorKind { get; init; } = TextCursorKind.Beam;
}