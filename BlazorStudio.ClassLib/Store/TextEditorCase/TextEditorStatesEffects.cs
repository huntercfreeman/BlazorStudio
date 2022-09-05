using System.Collections.Concurrent;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

public class TextEditorStatesEffects
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IState<TextEditorStates> _textEditorStatesWrap;
    private ConcurrentDictionary<AbsoluteFilePathStringValue, string> ;

    public TextEditorStatesEffects(IFileSystemProvider fileSystemProvider,
        IState<TextEditorStates> textEditorStatesWrap)
    {
        _fileSystemProvider = fileSystemProvider;
        _textEditorStatesWrap = textEditorStatesWrap;
    }
    
    [EffectMethod]
    public async Task HandleRequestSaveTextEditorAction(RequestSaveTextEditorAction requestSaveTextEditorAction,
        IDispatcher dispatcher)
    {
        var saveTarget = _textEditorStatesWrap.Value.TextEditors
            .FirstOrDefault(x =>
                x.TextEditorKey == requestSaveTextEditorAction.TextEditorKey);

        if (saveTarget is null)
            return;

        var contentToWriteOut = saveTarget.GetAllText();
        
        dispatcher.Dispatch(new );
    }
}