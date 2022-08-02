using System.Collections.Immutable;
using System.IO.MemoryMappedFiles;
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
    /// <summary>
    /// The PlainTextEditor needs an abstraction of a modifiable <see cref="MemoryMappedFile"/>
    /// <br/><br/>
    /// Idea 1: use <see cref="MemoryMappedFile"/> to not read the entirety of a large file into memory.
    /// <br/><br/>
    /// Idea 2.a: if one does not read a file into memory, how does one edit the document? When the user
    /// makes an edit to line 1 of the file, then proceeds to scroll to line 1,000,000 the cache
    /// would no longer have line 1 in memory. So, use <see cref="IFileHandle"/> to maintain a list of edits to a physical file.
    /// <br/><br/>
    /// Idea 2.b: So, use <see cref="IFileHandle"/> to maintain a list of edits to a physical file. Then when reading
    /// the physical file iterate over the list of edits to effectively maintain a 'virtual file'. This results in
    /// one maintaining edits a user makes instead of the entirety of a large file's contents. When the user saves
    /// the file clear the list of edits.
    /// </summary>
    public IFileHandle? FileHandle { get; }
    public RichTextEditorOptions RichTextEditorOptions { get; }
    /// <summary>
    /// When the PlainTextEditor is rendered to a user interface it is done so
    /// with vertical, and horizontal virtualization to avoid rendering the entire
    /// file and making the UI unresponsive.
    /// <br/><br/>
    /// This property is maintained here because the PlainTextEditor is immutable. Therefore
    /// there is a direct relationship between the <see cref="VirtualizeCoordinateSystemMessage"/>
    /// and the current state of the <see cref="IPlainTextEditor"/>. This direct relationship
    /// can be useful to look at when debugging. Although this property is not required for functionality of the PlainTextEditor.
    /// </summary>
    public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get; }
    /// <summary>
    /// There is a discrepancy between what the <see cref="VirtualizeCoordinateSystemRequest"/>
    /// asks for and what the <see cref="FileHandleReadRequest"/> will ultimately ask for.
    /// <br/>br/>
    /// Example: the user interface might say, "give me rows 1 to 100 and columns 1 to 50 because
    /// that's what the user can see currently (including some padding)." But, for efficiency purposes the
    /// <see cref="FileHandleReadRequest"/> might scale the request to read in more than requested to avoid
    /// frequent filesystem operations.
    /// </summary>
    public FileHandleReadRequest FileHandleReadRequest { get; init; }
    /// <summary>
    /// When this is true the PlainTextEditor will ignore any action the User
    /// takes that would result in a file modification.
    /// <br/><br/>
    /// Example, the keyboard event of hitting the 'ArrowDown' key will function as expected.
    /// However, the keyboard event of hitting the 'a' key will be ignored.
    /// </summary>
    public bool IsReadonly { get; }
    /// <summary>
    /// The PlainTextEditor parses the entirety of the physical file that is targeted
    /// for any "\r\n" strings. If it finds one this is set to True.
    /// <br/><br/>
    /// When this is set to True, any keyboard events of from hitting the 'Enter' key
    /// will result in "\r\n" instead of "\n" in order to maintain the line endings in a consistent manner.
    /// <br/><br/>
    /// When this is set to False any keyboard events of from hitting the 'Enter' key
    /// will result in "\n" 
    /// </summary>
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
