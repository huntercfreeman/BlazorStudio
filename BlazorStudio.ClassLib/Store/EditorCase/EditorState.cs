using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
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

    public static Task ShowInputFileAsync(
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        TextEditorResourceMapState textEditorResourceMapState)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "TextEditor",
                async afp =>
                {
                    await OpenInEditorAsync(
                        afp, 
                        dispatcher, 
                        textEditorService, 
                        textEditorResourceMapState);
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
    
    public static async Task OpenInEditorAsync(
        IAbsoluteFilePath? absoluteFilePath,
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        TextEditorResourceMapState textEditorResourceMapState)
    {
        if (absoluteFilePath is null)
        {
            dispatcher.Dispatch(
                new SetActiveTextEditorKeyAction(null));

            return;
        }

        if (absoluteFilePath.IsDirectory)
            return;
        
        var inputFileAbsoluteFilePathString = absoluteFilePath.GetAbsoluteFilePathString();

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

        // If a text editor for the file used to exist but
        // the resource was not disposed of
        if (existingResourceMapPair is not null && 
            textEditorService.TextEditorStates.TextEditorList
                .All(x => 
                    x.Key != existingResourceMapPair.Value.Key))
        {
            dispatcher.Dispatch(
                new TextEditorResourceMapState.RemoveTextEditorResourceAction(
                    existingResourceMapPair.Value.Key!));

            // Set existingResourceMapPair to null
            // so the next if statement outside of this block
            // will create a new text editor
            existingResourceMapPair = null;
        }
        
        // If a new text editor is needed
        if (existingResourceMapPair is null)
        {
            var content = await File
                .ReadAllTextAsync(inputFileAbsoluteFilePathString);

            var textEditor = new TextEditorBase(
                content,
                ExtensionNoPeriodFacts.GetLexer(absoluteFilePath.ExtensionNoPeriod),
                ExtensionNoPeriodFacts.GetDecorationMapper(absoluteFilePath.ExtensionNoPeriod),
                null,
                textEditorKey
            );
            
            dispatcher.Dispatch(
                new TextEditorResourceMapState.SetTextEditorResourceAction(
                    textEditorKey,
                    absoluteFilePath));

            await textEditor.ApplySyntaxHighlightingAsync();
            
            textEditorService.RegisterTextEditor(
                textEditor);
        }

        dispatcher.Dispatch(
            new SetActiveTextEditorKeyAction(textEditorKey));
    }
}