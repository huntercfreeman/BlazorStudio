using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.TextEditorResourceMapCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

[FeatureState]
public record EditorState(TextEditorKey? ActiveTextEditorKey)
{
    public EditorState() : this(TextEditorKey.Empty)
    {
    }

    public record SetActiveTextEditorKeyAction(TextEditorKey? TextEditorKey);

    public class EditorStateReducer
    {
        [ReducerMethod]
        public static EditorState ReduceSetActiveTextEditorKeyAction(
            EditorState inEditorState,
            SetActiveTextEditorKeyAction setActiveTextEditorKeyAction)
        {
            return inEditorState with
            {
                ActiveTextEditorKey = setActiveTextEditorKeyAction.TextEditorKey
            };
        }
    }

    public static Task OpenFileOnClick(
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        TextEditorResourceMapState textEditorResourceMapState)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "TextEditor",
                async afp =>
                {
                    if (afp is null)
                    {
                        dispatcher.Dispatch(
                            new SetActiveTextEditorKeyAction(null));

                        return;
                    }

                    var inputFileAbsoluteFilePathString = afp.GetAbsoluteFilePathString();

                    KeyValuePair<TextEditorKey, IAbsoluteFilePath>? existingResourceMapPair = null;

                    foreach (var kvp in textEditorResourceMapState.ResourceMap)
                    {
                        if (kvp.Value.GetAbsoluteFilePathString() == inputFileAbsoluteFilePathString)
                        {
                            existingResourceMapPair = kvp;
                        }
                    }

                    var textEditorKey = existingResourceMapPair?.Key ?? 
                                        TextEditorKey.NewTextEditorKey();

                    if (existingResourceMapPair is null)
                    {
                        var content = await File
                            .ReadAllTextAsync(inputFileAbsoluteFilePathString);

                        textEditorService.RegisterTextEditor(
                            new TextEditorBase(
                                content,
                                null,
                                null,
                                null,
                                textEditorKey
                            ));
                    }

                    dispatcher.Dispatch(
                        new SetActiveTextEditorKeyAction(textEditorKey));
                },
                afp =>
                {
                    if (afp is null ||
                        afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(true);
                },
                new[]
                {
                    new InputFilePattern(
                        "File",
                        afp => !afp.IsDirectory)
                }.ToImmutableArray()));

        return Task.CompletedTask;
    }
}