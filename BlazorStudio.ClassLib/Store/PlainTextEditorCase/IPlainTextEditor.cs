using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public interface IPlainTextEditor
{
    public PlainTextEditorKey PlainTextEditorKey { get; } 
    public SequenceKey SequenceKey { get; } 
    public ImmutableList<IPlainTextEditorRow> List { get; }
    public int CurrentRowIndex { get; }
    public int CurrentTokenIndex { get; }
    public IFileHandle? FileHandle { get; }
    public RichTextEditorOptions RichTextEditorOptions { get; }
    public int LongestRowCharacterLength { get; }
    public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get; }
    public FileHandleReadRequest FileHandleReadRequest { get; init; }
    public int CharacterIndexOffsetRelativeToRow { get; }
    public bool IsReadonly { get; }
    public bool UseCarriageReturnNewLine { get; }

    // New things

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
    public int ColumnIndexOffset { get; }

    public string GetPlainText();
}
