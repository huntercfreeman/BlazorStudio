using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TextEditor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

/// <summary>
/// Either returns a constructed TextEditor or returns
/// an existing TextEditor if there already exists one
/// for the given physical file.
/// <br/><br/>
/// In the case that there already exists one
/// for the given physical file the two requesters of the TextEditor can
/// have different viewports (scroll positions etc...) due to <see cref="TextPartition"/>
/// </summary>
public record RequestConstructTextEditorAction(
    TextEditorKey TextEditorKey, 
    IAbsoluteFilePath AbsoluteFilePath, 
    string Content,
    Func<string, CancellationToken, Task> OnSaveRequestedFuncAsync,
    Func<EventHandler> GetInstanceOfPhysicalFileWatcherFunc);