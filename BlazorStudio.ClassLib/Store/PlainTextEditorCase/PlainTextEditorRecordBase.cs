using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public abstract record PlainTextEditorRecordBase(PlainTextEditorKey PlainTextEditorKey,
    SequenceKey SequenceKey,
    ImmutableList<IPlainTextEditorRow> Rows,
    int CurrentRowIndex,
    int CurrentTokenIndex,
    int CurrentColumnIndex,
    RichTextEditorOptions RichTextEditorOptions,
    bool IsReadonly = true,
    bool UseCarriageReturnNewLine = false) : IPlainTextEditor
{
    public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get;  init; }
    public FileHandleReadRequest FileHandleReadRequest { get; init; }
    public int CurrentCharacterColumnIndex { get;  init; }
    public int PreviouslySetCharacterColumnIndex { get;  init; }
    public int RowIndexOffset { get;  init; }
    public int CharacterColumnIndexOffset { get;  init; }
    public SelectionSpanRecord? SelectionSpan { get;  init; }
    public IFileHandle FileHandle { get;  init; }
    public long CurrentPositionIndex { get; init; }
    
    public abstract IAbsoluteFilePath? AbsoluteFilePath { get; }
    public abstract IPlainTextEditorRow CurrentPlainTextEditorRow { get; }
    public abstract TextTokenKey CurrentTextTokenKey { get; }
    public abstract ITextToken CurrentTextToken { get; }
    public abstract PlainTextEditorKind PlainTextEditorKind { get; }

    public T GetCurrentTextTokenAs<T>()
        where T : class
    {
        return CurrentTextToken as T
               ?? throw new ApplicationException($"Expected {typeof(T).Name}");
    }

    /// <summary>
    /// TODO: Remove this and in its place use <see cref="ConvertIPlainTextEditorRowAs"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public T GetCurrentPlainTextEditorRowAs<T>()
        where T : class
    {
        return CurrentPlainTextEditorRow as T
               ?? throw new ApplicationException($"Expected {typeof(T).Name}");
    }

    public T ConvertIPlainTextEditorRowAs<T>(IPlainTextEditorRow plainTextEditorRow)
        where T : class
    {
        return plainTextEditorRow as T
               ?? throw new ApplicationException($"Expected {typeof(T).Name}");
    }

    public abstract IPlainTextEditorRow GetEmptyPlainTextEditorRow();
    public abstract IPlainTextEditorRow GetWidthAndHeightTestPlainTextEditorRow();
    public abstract string GetPlainText();
}