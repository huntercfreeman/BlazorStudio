using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public interface IPlainTextEditor
{
    /// <summary>
    /// A unique identifier for a given <see cref="IPlainTextEditor"/>
    /// </summary>
    public PlainTextEditorKey PlainTextEditorKey { get; } 
    /// <summary>
    /// 
    /// </summary>
    public SequenceKey SequenceKey { get; } 
    public ImmutableList<IPlainTextEditorRow> List { get; }
    public IFileHandle? FileHandle { get; }
    public RichTextEditorOptions RichTextEditorOptions { get; }
    public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get; }
    public FileHandleReadRequest FileHandleReadRequest { get; init; }
    public bool IsReadonly { get; }
    public bool UseCarriageReturnNewLine { get; }
    /// <summary>
    /// This is the row index relative to the small section of a larger file
    /// that was read into a PlainTextEditor.
    ///
    /// To get the row index relative to the actual file one must
    /// add the <see cref="RowIndexOffset"/>
    /// </summary>
    public int CurrentRowIndex { get; }
    /// <summary>
    /// This is the index of the active token relative to the current row.
    ///
    /// Not to be confused with <see cref="CurrentCharacterColumnIndex"/>
    /// </summary>
    public int CurrentTokenIndex { get; }
    /// <summary>
    /// This is the index of the primary cursor relative to the current row.
    /// </summary>
    public int CurrentCharacterColumnIndex { get; }
    /// <summary>
    /// This is the index of the primary cursor relative to the flat entirety of
    /// the file itself (no rows involved).
    /// </summary>
    public int CurrentPositionIndex { get; }
    /// <summary>
    /// Similar to the <see cref="CurrentCharacterColumnIndex"/> this is
    /// the index of the primary cursor relative to the current row.
    /// <br/><br/>
    /// However, this is mostly only set when the user hits on their keyboard the keys:
    /// 'ArrowLeft', 'ArrowRight'.
    /// <br/><br/>
    /// The usage of this is for when the user hits on their keyboard the keys:
    /// 'ArrowUp', 'ArrowDown'.
    /// <br/><br/>
    /// 'ArrowUp', 'ArrowDown' will set <see cref="CurrentCharacterColumnIndex"/> to the minimum
    /// of <see cref="PreviouslySetCharacterColumnIndex"/> and the maximum character
    /// column index of the row traveled to.
    /// </summary>
    public int PreviouslySetCharacterColumnIndex { get; }
    /// <summary>
    /// The PlainTextEditor only loads in small sections of a larger file.
    /// <br/><br/>
    /// For a large file of 100,000 lines of text if one is only viewing
    /// lines 5,000 to 5,999 then the PlainTextEditor would hold in memory
    /// a representation of those 1,000 lines only.
    /// <br/><br/>
    /// In order to sync the indices of the PlainTextEditor's rows and the
    /// actual indices of the row in the file an offset must be stored.
    /// <br/><br/>
    /// In the previous example the <see cref="RowIndexOffset"/> would be 4,999
    /// </summary>
    public int RowIndexOffset { get; }
    /// <summary>
    /// The PlainTextEditor only loads in small sections of a larger file.
    /// <br/><br/>
    /// For a large file of 100,000 lines of text if one is only viewing
    /// character columns 100 to 199 then the PlainTextEditor would hold in memory
    /// a representation of those 100 to 199 columns only (the amount of rows to read
    /// the columns of is defined in part by: <see cref="RowIndexOffset"/>
    /// <br/><br/>
    /// In order to sync the indices of the PlainTextEditor's columns and the
    /// actual indices of the column in the file an offset must be stored.
    /// <br/><br/>
    /// In the previous example the <see cref="RowIndexOffset"/> would be 99
    /// </summary>
    public int CharacterColumnIndexOffset { get; }

    public string GetPlainText();
}
