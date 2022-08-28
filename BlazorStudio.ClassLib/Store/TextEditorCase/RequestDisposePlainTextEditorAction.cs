using BlazorStudio.ClassLib.TextEditor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

/// <summary>
/// Decreases the amount of <see cref="TextPartition"/>(s) in use for
/// the <see cref="TextEditorBase"/> with the corresponding <see cref="TextEditorKey"/>
/// <br/><br/>
/// If <see cref="TextPartition"/>(s) is empty after this action then the TextEditor
/// will be disposed.
/// </summary>
public record RequestDisposePlainTextEditorAction(TextEditorKey TextEditorKey);