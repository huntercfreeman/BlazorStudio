using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(
    ImmutableDictionary<TextEditorKey, TextEditorBase> TextEditorMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, TextEditorKey> AbsoluteFilePathToActiveTextEditorMap)
{
    public TextEditorStates() : this(
        ImmutableDictionary<TextEditorKey, TextEditorBase>.Empty,
        ImmutableDictionary<AbsoluteFilePathStringValue, TextEditorKey>.Empty)
    {
        
    }
}

public class TextEditorStatesReducer
{
    [ReducerMethod]
    public static TextEditorStates ReduceRequestConstructTextEditorAction(TextEditorStates previousTextEditorStates,
        RequestConstructTextEditorAction requestConstructTextEditorAction)
    {
        var absoluteFilePathStringValue = new AbsoluteFilePathStringValue(requestConstructTextEditorAction.AbsoluteFilePath);
        
        if (!previousTextEditorStates.AbsoluteFilePathToActiveTextEditorMap
            .ContainsKey(absoluteFilePathStringValue))
        {
            var constructedTextEditor = new TextEditorBase(
                requestConstructTextEditorAction.TextEditorKey,
                requestConstructTextEditorAction.Content,
                requestConstructTextEditorAction.AbsoluteFilePath,
                requestConstructTextEditorAction.OnSaveRequestedFuncAsync,
                requestConstructTextEditorAction.GetInstanceOfPhysicalFileWatcherFunc);


            var nextAbsoluteFilePathToActiveTextEditorMap = 
                previousTextEditorStates.AbsoluteFilePathToActiveTextEditorMap
                    .SetItem(
                        absoluteFilePathStringValue,
                        constructedTextEditor.TextEditorKey);

            var nextTextEditorMap =
                previousTextEditorStates.TextEditorMap
                    .SetItem(
                        constructedTextEditor.TextEditorKey,
                        constructedTextEditor);

            return new TextEditorStates(
                nextTextEditorMap,
                nextAbsoluteFilePathToActiveTextEditorMap);
        }

        return previousTextEditorStates;
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceRequestDisposePlainTextEditorAction(TextEditorStates previousTextEditorStates,
        RequestDisposePlainTextEditorAction requestDisposePlainTextEditorAction)
    {
        throw new NotImplementedException();
    }
}