using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

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
    public static TextEditorStates ReduceTextEditorEditAction(TextEditorStates previousTextEditorStates,
        TextEditorEditAction textEditorEditAction)
    {
        var textEditor = previousTextEditorStates.TextEditorMap[textEditorEditAction.TextEditorKey];

        var nextTextEditor = textEditor.PerformTextEditorEditAction(textEditorEditAction);

        var nextMap = previousTextEditorStates.TextEditorMap
            .SetItem(textEditorEditAction.TextEditorKey, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorMap = nextMap
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceRequestDisposePlainTextEditorAction(TextEditorStates previousTextEditorStates,
        RequestDisposePlainTextEditorAction requestDisposePlainTextEditorAction)
    {
        throw new NotImplementedException();
    }
}