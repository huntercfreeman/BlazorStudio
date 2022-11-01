using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorResourceMapCase;

[FeatureState]
public record TextEditorResourceMapState(ImmutableDictionary<TextEditorKey, IAbsoluteFilePath> ResourceMap)
{
    public TextEditorResourceMapState() : this(ImmutableDictionary<TextEditorKey, IAbsoluteFilePath>.Empty)
    {
    }

    public record SetTextEditorResourceAction(TextEditorKey TextEditorKey, IAbsoluteFilePath AbsoluteFilePath);
    public record RemoveTextEditorResourceAction(TextEditorKey TextEditorKey);

    private class TextEditorResourceMapStateReducer
    {
        [ReducerMethod]
        public static TextEditorResourceMapState ReduceSetTextEditorResourceAction(
            TextEditorResourceMapState inTextEditorResourceMapState,
            SetTextEditorResourceAction setTextEditorResourceAction)
        {
            var nextMap = inTextEditorResourceMapState.ResourceMap
                .Add(
                    setTextEditorResourceAction.TextEditorKey,
                    setTextEditorResourceAction.AbsoluteFilePath);

            return inTextEditorResourceMapState with
            {
                ResourceMap = nextMap
            };
        }
        
        [ReducerMethod]
        public static TextEditorResourceMapState ReduceRemoveTextEditorResourceAction(
            TextEditorResourceMapState inTextEditorResourceMapState,
            RemoveTextEditorResourceAction removeTextEditorResourceAction)
        {
            var nextMap = inTextEditorResourceMapState.ResourceMap
                .Remove(removeTextEditorResourceAction.TextEditorKey);

            return inTextEditorResourceMapState with
            {
                ResourceMap = nextMap
            };
        }
    }
}