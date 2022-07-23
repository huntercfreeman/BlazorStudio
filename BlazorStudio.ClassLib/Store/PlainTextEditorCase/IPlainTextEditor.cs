using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
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
    public IFileCoordinateGrid? FileCoordinateGrid { get; }
    public RichTextEditorOptions RichTextEditorOptions { get; }
    public int LongestRowCharacterLength { get; }
    public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get; }
    public int RowIndexOffset { get; }

    public string GetPlainText();
}
