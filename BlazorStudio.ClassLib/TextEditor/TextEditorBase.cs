using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
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
    private readonly ImmutableArray<TextCharacter> _content;

    private readonly IAbsoluteFilePath _absoluteFilePath;
    private readonly Func<string, CancellationToken, Task> _onSaveRequestedFuncAsync;
    private readonly Func<EventHandler> _getInstanceOfPhysicalFileWatcherFunc;

    private readonly List<TextPartition> _activeTextPartitions = new();
    private readonly List<int> _lineEndingPositions = new();
    
    private EventHandler? _physicalFileWatcher;

    public TextEditorBase(
        TextEditorKey textEditorKey,
        string content,
        IAbsoluteFilePath absoluteFilePath,
        Func<string, CancellationToken, Task> onSaveRequestedFuncAsync,
        Func<EventHandler> getInstanceOfPhysicalFileWatcherFuncFunc)
    {
        TextEditorKey = textEditorKey;

        var previousKey = string.Empty;

        var rowIndex = 0;
        
        _content = content.Select((character, index) =>
        {
            var key = character.ToString();

                // '\r' and '\r\n'
            if ((KeyboardKeyFacts.NewLineKeys.CARRIAGE_RETURN_KEY == key) ||
                // '\n'
                (KeyboardKeyFacts.NewLineKeys.NEW_LINE_KEY == key && previousKey != KeyboardKeyFacts.NewLineKeys.CARRIAGE_RETURN_KEY))
            {
                // Track line ending position and
                // increment row index
                {
                    _lineEndingPositions.Add(index + 1);
                    rowIndex++;
                }
            }

            previousKey = key;
            
            return new TextCharacter
            {
                Value = character,
                DecorationByte = default
            };   
        }).ToImmutableArray();

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
                    .ToImmutableArray()
            };
            
            textSpanRows.Add(textCharacterSpan);
        }
        
        return new TextPartition(
            TextEditorLink.Empty(), 
            rectangularCoordinates,
            textSpanRows.ToImmutableArray(),
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
                _content[i].DecorationByte = (byte) decorationKind;
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