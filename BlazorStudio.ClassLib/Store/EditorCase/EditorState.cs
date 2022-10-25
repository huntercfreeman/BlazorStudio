using BlazorTextEditor.RazorLib.MoveThese;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

/// <summary>
/// As of this comment <see cref="EditorState"/> represents the Active file tab 
/// </summary>
/// <param name="TextEditorKey"></param>
[FeatureState]
public record EditorState(TextEditorKey TextEditorKey)
{
    public EditorState() : this(TextEditorKey.Empty)
    {

    }
}