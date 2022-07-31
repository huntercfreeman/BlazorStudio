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
    public int RowIndexOffset { get; }
    public int CharacterIndexOffsetRelativeToRow { get; }
    public bool IsReadonly { get; }

    public string GetPlainText();
}
