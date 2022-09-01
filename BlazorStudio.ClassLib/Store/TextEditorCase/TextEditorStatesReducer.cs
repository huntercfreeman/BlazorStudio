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
                previousTextEditorStates.TextEditors.Add(constructedTextEditor);

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
        var textEditor = previousTextEditorStates.TextEditors
            .Single(x => x.TextEditorKey == textEditorEditAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformTextEditorEditAction(textEditorEditAction);

        var nextMap = previousTextEditorStates.TextEditors
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditors = nextMap
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceRequestDisposePlainTextEditorAction(TextEditorStates previousTextEditorStates,
        RequestDisposePlainTextEditorAction requestDisposePlainTextEditorAction)
    {
        var disposeTarget = previousTextEditorStates.TextEditors
            .FirstOrDefault(x =>
                x.TextEditorKey == requestDisposePlainTextEditorAction.TextEditorKey);

        if (disposeTarget is null)
            return previousTextEditorStates;
        
        var nextAbsoluteFilePathToActiveTextEditorMap = 
            previousTextEditorStates.AbsoluteFilePathToActiveTextEditorMap
                .Remove(new (disposeTarget.AbsoluteFilePath));

        var nextTextEditorMap =
            previousTextEditorStates.TextEditors.Remove(disposeTarget);

        return new TextEditorStates(
            nextTextEditorMap,
            nextAbsoluteFilePathToActiveTextEditorMap);
    }
}