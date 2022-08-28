using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TextEditor.Character;

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
    
    private EventHandler? _physicalFileWatcher;

    public TextEditorBase(
        TextEditorKey textEditorKey,
        string content,
        IAbsoluteFilePath absoluteFilePath,
        Func<string, CancellationToken, Task> onSaveRequestedFuncAsync,
        Func<EventHandler> getInstanceOfPhysicalFileWatcherFuncFunc)
    {
        TextEditorKey = textEditorKey;
        
        _content = content.Select(x => new TextCharacter
        {
            Value = x,
            Decoration = default
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
    /// The property <see cref="TextPartitions"/> is an <see cref="ImmutableList"/> as opposed to a singular
    /// <see cref="TextPartition"/>. This is because a user might open the same file in separate windows.
    /// Each <see cref="TextPartition"/> would be an 'active link' to the file.
    /// <br/>--<br/>
    /// When all 'active link'(s) are gone then the TextEditorBase is disposed of.
    /// </summary>
    public ImmutableList<TextPartition> TextPartitions { get; set; }
    /// <summary>
    /// Unique identifier for a <see cref="TextEditorBase"/>
    /// </summary>
    public TextEditorKey TextEditorKey { get; init; } = TextEditorKey.NewTextEditorKey();
    
    /// <summary>
    /// When the physical file has changes saved to it (whether that be from a different process
    /// or not) a notification is sent.
    /// </summary>
    public void WatchPhysicalFile()
    {
        _physicalFileWatcher = _getInstanceOfPhysicalFileWatcherFunc.Invoke();

        _physicalFileWatcher += OnPhysicalFileChanged;
    }
    
    /// <summary>
    /// Overwrite the contents of the physical file with the edited version.
    /// </summary>
    public async Task SaveToPhysicalFileAsync(CancellationToken cancellationToken = default)
    {
        await _onSaveRequestedFuncAsync.Invoke(
            new string(_content.Select(x => x.Value).ToArray()), 
            cancellationToken);
    }

    private void OnPhysicalFileChanged(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
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