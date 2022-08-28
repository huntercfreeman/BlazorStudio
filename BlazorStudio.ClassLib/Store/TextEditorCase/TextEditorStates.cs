using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TextEditor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

public record TextEditorStates(
    ImmutableDictionary<TextEditorKey, TextEditorBase> TextEditorMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, TextEditorKey> AbsoluteFilePathToActiveTextEditorMap);

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
    Func<string, CancellationToken, Task> OnSaveRequestedFuncAsync,
    Func<EventHandler> GetInstanceOfPhysicalFileWatcherFunc);

/// <summary>
/// Decreases the amount of <see cref="TextPartition"/>(s) in use for
/// the <see cref="TextEditorBase"/> with the corresponding <see cref="TextEditorKey"/>
/// <br/><br/>
/// If <see cref="TextPartition"/>(s) is empty after this action then the TextEditor
/// will be disposed.
/// </summary>
public record RequestDisposePlainTextEditorAction(TextEditorKey TextEditorKey);