using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor.Character;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using Microsoft.CodeAnalysis.Text;

namespace BlazorStudio.ClassLib.TextEditor;

public record TextEditorBase : IDisposable
{
    /// <summary>
    /// Thank you to "A Crawford" who commented on my youtube video:
    /// https://youtu.be/kjpx_rN8hTQ
    /// <br/>
    /// "I don't even think word only partly loads the file, it creates a copy in memory"
    /// I am going to give this a go and <see cref="_content"/> is the copy in memory.
    /// </summary>
    private readonly List<TextCharacter> _content;

    private readonly IAbsoluteFilePath _absoluteFilePath;
    private readonly Func<string, CancellationToken, Task> _onSaveRequestedFuncAsync;
    private readonly Func<EventHandler> _getInstanceOfPhysicalFileWatcherFunc;

    /// <summary>
    /// The UserInterface should not be immutable it is far too resource intensive and slow.
    /// <br/><br/>
    /// Therefore the UserInterface receives mutable <see cref="TextPartition"/>.
    /// <br/><br/>
    /// The <see cref="TextEditorBase"/> is the 'all knowing, singular source of truth'
    /// and can then mutate what is displayed on
    /// the user interface by modifying an active text partition and then sending out a rerender notification.
    /// <br/><br/>
    /// <see cref="_content"/> is a mutable List however all public API that use it cannot mutate the
    /// data. <see cref="TextCharacter.DecorationByte"/> is mutable but that is used for UserInterface only.
    /// <br/><br/>
    /// What matters is that <see cref="_content"/> is not exposed for outside mutation and that
    /// the underlying characters <see cref="TextCharacter.Value"/> is immutable. 
    /// </summary>
    private readonly List<TextPartition> _activeTextPartitions = new();
    // Returns the first inclusive character position of the row that may or may not follow (possibly End of File)
    private readonly List<int> _lineEndingPositions = new();
    private List<IEditBlock> _editBlocks = new();
    
    private EventHandler? _physicalFileWatcher;

    public TextEditorBase(
        TextEditorKey textEditorKey,
        string content,
        IAbsoluteFilePath absoluteFilePath,
        Func<string, CancellationToken, Task> onSaveRequestedFuncAsync,
        Func<EventHandler> getInstanceOfPhysicalFileWatcherFuncFunc)
    {
        TextEditorKey = textEditorKey;

        var previousKey = ' ';

        var rowIndex = 0;
        
        _content = content.Select((character, index) =>
        {
                // '\r' and '\r\n'
            if ((KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN == character) ||
                // '\n'
                (KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE == character && previousKey != KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN))
            {
                // Track line ending position and
                // increment row index
                {
                    _lineEndingPositions.Add(index + 1);
                    rowIndex++;
                }
            }

            previousKey = character;
            
            return new TextCharacter(character)
            {
                DecorationByte = default
            };   
        }).ToList();

        _absoluteFilePath = absoluteFilePath;
        _onSaveRequestedFuncAsync = onSaveRequestedFuncAsync;
        _getInstanceOfPhysicalFileWatcherFunc = getInstanceOfPhysicalFileWatcherFuncFunc;
    }

    /// <summary>
    /// The end user will (likely) not have an entire file rendered on the UI at a given point in time.
    /// <br/>--<br/>
    /// The Type <see cref="TextPartition"/> is used to represent a given viewport's text content. (And some padding
    /// as is typical with Virtualization rendering techniques).
    /// <br/>--<br/>
    /// The property <see cref="ActiveTextPartitions"/> is an <see cref="ImmutableList"/> as opposed to a singular
    /// <see cref="TextPartition"/>. This is because a user might open the same file in separate windows.
    /// Each <see cref="TextPartition"/> would be an 'active link' to the file.
    /// <br/>--<br/>
    /// When all 'active link'(s) are gone then the TextEditorBase is disposed of.
    /// </summary>
    public ImmutableArray<TextPartition> ActiveTextPartitions => _activeTextPartitions.ToImmutableArray();
    /// <summary>
    /// Unique identifier for a <see cref="TextEditorBase"/>
    /// </summary>
    public TextEditorKey TextEditorKey { get; init; } = TextEditorKey.NewTextEditorKey();
    public ImmutableArray<int> LineEndingPositions => _lineEndingPositions.ToImmutableArray();
    
    /// <summary>
    /// When the physical file has changes saved to it (whether that be from a different process
    /// or not) a notification is sent.
    /// </summary>
    public void WatchPhysicalFile()
    {
        _physicalFileWatcher = _getInstanceOfPhysicalFileWatcherFunc.Invoke();

        _physicalFileWatcher += OnPhysicalFileChanged;
    }

    public string GetAllText()
    {
        return new string(_content.Select(x => x.Value).ToArray());
    }
    
    public TextPartition GetTextPartition(RectangularCoordinates rectangularCoordinates)
    {
        var rowCountRequested = rectangularCoordinates.BottomRightCorner.RowIndex.Value -
                            rectangularCoordinates.TopLeftCorner.RowIndex.Value;

        var rowCountAvailable = _lineEndingPositions.Count -
                                rectangularCoordinates.TopLeftCorner.RowIndex.Value;

        var rowCount = rowCountRequested < rowCountAvailable
            ? rowCountRequested
            : rowCountAvailable;

        var endingRowIndexExclusive = rectangularCoordinates.TopLeftCorner.RowIndex.Value + rowCount;

        var textSpanRows = new List<TextCharacterSpan>();
        
        for (int i = rectangularCoordinates.TopLeftCorner.RowIndex.Value; 
             i < endingRowIndexExclusive;
             i++)
        {
            // Previous row's line ending position is this row's start.
            //
            // TODO: (need to figure out '\r\n' vs '\r' vs '\n' etc as I might
            // have to add either 2 characters to the position or 1 characters depending
            // on the line ending character's length)
            var startOfTextSpanRowInclusive = i == 0
                ? 0
                : _lineEndingPositions[i - 1];

            var endOfTextSpanRowExclusive = _lineEndingPositions[i];

            var textCharacterSpan = new TextCharacterSpan
            {
                Start = startOfTextSpanRowInclusive,
                End = _lineEndingPositions[i],
                TextCharacters = _content
                    .Skip(startOfTextSpanRowInclusive)
                    .Take(endOfTextSpanRowExclusive - startOfTextSpanRowInclusive)
                    .ToList()
            };
            
            textSpanRows.Add(textCharacterSpan);
        }
        
        return new TextPartition(
            TextEditorLink.Empty(), 
            rectangularCoordinates,
            textSpanRows.ToList(),
            SequenceKey.NewSequenceKey());
    }
    
    /// <summary>
    /// Overwrite the contents of the physical file with the edited version.
    /// </summary>
    public async Task SaveToPhysicalFileAsync(CancellationToken cancellationToken = default)
    {
        await _onSaveRequestedFuncAsync.Invoke(
            GetAllText(), 
            cancellationToken);
    }

    private void OnPhysicalFileChanged(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public void ApplyDecorationRange(DecorationKind decorationKind, IEnumerable<TextSpan> textSpans)
    {
        foreach (var textSpan in textSpans)
        {
            for (int i = textSpan.Start; i < textSpan.End; i++)
            {
                _content[i].DecorationByte = (byte)decorationKind;
            }
        }
    }
    
    public static int GetLengthOfRow(RowIndex rowIndex, ImmutableArray<int> lineEndingPositions)
    {
        var startOfTextSpanRowInclusive = rowIndex.Value == 0
            ? 0
            : lineEndingPositions[rowIndex.Value - 1];

        var endOfTextSpanRowExclusive =
            lineEndingPositions[rowIndex.Value];

        return endOfTextSpanRowExclusive - startOfTextSpanRowInclusive;
    }
    
    public static TextCharacterKind GetTextCharacterKind(TextCharacter textCharacter)
    {
        if (KeyboardKeyFacts.IsPunctuationCharacter(textCharacter.Value))
            return TextCharacterKind.Punctuation;
        
        if (KeyboardKeyFacts.IsWhitespaceCharacter(textCharacter.Value))
            return TextCharacterKind.Whitespace;

        return TextCharacterKind.Plain;
    }
    
    /// <summary>
    /// Find the closest <see cref="TextCharacter"/> that has a
    /// <see cref="TextCharacterKind"/> != the passed parameter's <see cref="TextCharacterKind"/> 
    /// </summary>
    /// <param name="lookBackwards">The search will go to the left of the passed parameter if true otherwise it will search to the right.</param>
    public ColumnIndex ClosestNonMatchingCharacterOnSameRowColumnIndex(
        RowIndex rowIndex, 
        ColumnIndex columnIndex, 
        bool lookBackwards)
    {
        // Ensure the caller's state is not mutated
        rowIndex = new RowIndex(rowIndex);
        columnIndex = new ColumnIndex(columnIndex);
        
        void MutateLocalColumnIndex()
        {
            if (lookBackwards)
                columnIndex.Value--;
            else
                columnIndex.Value++;
        }

        var startOfRow = rowIndex.Value > 0
            ? _lineEndingPositions[rowIndex.Value - 1]
            : 0;
        var nextRowStart = _lineEndingPositions[rowIndex.Value];

        // Get the cursor's TextCharacter to start with.
        var textCharacter = _content[startOfRow + columnIndex.Value];
        var startingKind = GetTextCharacterKind(textCharacter);
        
        while (columnIndex.Value > -1 &&
               columnIndex.Value < nextRowStart &&
               GetTextCharacterKind(textCharacter) == startingKind)
        {
            MutateLocalColumnIndex();
            textCharacter = _content[startOfRow + columnIndex.Value];
        }

        return columnIndex;
    }
    
    /// <summary>
    /// Immutability is not needed to prevent Concurrency timing issues.
    /// <br/><br/>
    /// This is because the state management library, "Fluxor" is being used
    /// with the [ReducerMethod] logic which is locked and synchronous.
    /// <br/><br/>
    /// That being said to maintain perfect Undo logic in the editor one must
    /// perform edits and maintain a variable of what the previous edit kind was.
    /// <br/><br/>
    /// Many insertions of text should NOT make an immutable version of the TextEditor's <see cref="_contents"/>
    /// <br/><br/>
    /// However, many insertions then a 'remove' operation like 'Backspace'
    /// one should prior to doing a different edit kind take an immutable snapshot of the <see cref="_contents"/>
    /// </summary>
    public TextEditorBase PerformTextEditorEditAction(
        TextEditorEditAction textEditorEditAction)
    {
        var textEditKind = TextEditKind.Other; 
        
        foreach (var textCursor in textEditorEditAction.ImmutableTextCursors)
        {
            if (KeyboardKeyFacts.IsMetaKey(textEditorEditAction.KeyboardEventArgs.Key) &&
                !KeyboardKeyFacts.IsWhitespaceCode(textEditorEditAction.KeyboardEventArgs.Code))
            {
                // TODO: Backspace and others
            }
            else
            {
                // TODO: This conditional branch is likely text insertion but I need to look for edge cases
                textEditKind = TextEditKind.Insertion;
                EnsureUndoPoint(textEditKind);
                
                var startOfRow = textCursor.IndexCoordinates.RowIndex.Value > 0
                    ? _lineEndingPositions[textCursor.IndexCoordinates.RowIndex.Value - 1]
                    : 0;

                // TODO: (This code block needs to be done as a complete transaction currently
                // content can be inserted then UI can ask for partition and have false line endings
                // and lastly then the line endings would be set correct but after the render.)
                {
                    var valueToInsert = textEditorEditAction.KeyboardEventArgs.Key.First();
                    
                    var previousEditBlock = _editBlocks.Last();
                    
                    if (previousEditBlock is EditBlock<StringBuilder> insertionEditBlock)
                    {
                        insertionEditBlock.TypedValue.Append(valueToInsert);
                    }
                    
                    _content.Insert(
                        startOfRow + textCursor.IndexCoordinates.ColumnIndex.Value,
                        new TextCharacter(valueToInsert)
                        {
                            DecorationByte = default
                        });

                    // TODO: (Updating _lineEndingPositions is likely able to done faster than this.
                    // I imagine the current way with this for loop could possibly get slow with files
                    // of many lines as each character insertion would run this.)
                    for (int i = textCursor.IndexCoordinates.RowIndex.Value; i < _lineEndingPositions.Count; i++)
                    {
                        _lineEndingPositions[i]++;
                    }
                }
            }
        }

        return this;
    }
    
    private void EnsureUndoPoint(TextEditKind textEditKind)
    {
        var previousEditBlock = _editBlocks.LastOrDefault();
        
        if (previousEditBlock is null ||
            previousEditBlock.TextEditKind != textEditKind)
        {
            if (textEditKind == TextEditKind.Insertion)
            {
                _editBlocks.Add(new EditBlock<StringBuilder>(
                    textEditKind, 
                    new(), 
                    GetAllText()));
            }
        }
    }
    
    private void ReleaseUnmanagedResources()
    {
        if (_physicalFileWatcher is not null)
        {
            _physicalFileWatcher -= OnPhysicalFileChanged;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~TextEditorBase()
    {
        Dispose(false);
    }
}