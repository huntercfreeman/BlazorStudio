using PlainTextEditor.ClassLib.Sequence;
using System.Collections.Immutable;
using BlazorStudio.Shared.FileSystem.Classes;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public interface IPlainTextEditor
{
    public PlainTextEditorKey PlainTextEditorKey { get; } 
    public SequenceKey SequenceKey { get; } 
    public ImmutableList<IPlainTextEditorRow> List { get; }
    public int CurrentRowIndex { get; }
    public int CurrentTokenIndex { get; }
    public IFileCoordinateGrid? FileCoordinateGrid { get; }
    public RichTextEditorOptions RichTextEditorOptions { get; }

    public string GetPlainText();
}
