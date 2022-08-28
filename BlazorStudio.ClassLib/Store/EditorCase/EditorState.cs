using BlazorStudio.ClassLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

[FeatureState]
public record EditorState(TextEditorKey TextEditorKey)
{
    public EditorState() : this(TextEditorKey.Empty())
    {

    }
}