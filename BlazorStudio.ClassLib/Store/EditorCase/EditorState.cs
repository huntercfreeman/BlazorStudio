using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.FileSystemCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.Group;
using BlazorTextEditor.RazorLib.Store.TextEditorCase.ViewModels;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

public class EditorState
{
    public static readonly TextEditorGroupKey EditorTextEditorGroupKey = TextEditorGroupKey.NewTextEditorGroupKey();
    
    public static Task ShowInputFileAsync(
        IDispatcher dispatcher,
        ITextEditorService textEditorService)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "TextEditor",
                async afp =>
                {
                    await OpenInEditorAsync(
                        afp, 
                        dispatcher, 
                        textEditorService);
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
        ITextEditorService textEditorService)
    {
        if (absoluteFilePath is null ||
            absoluteFilePath.IsDirectory)
        {
            return;
        }
        
        textEditorService.RegisterGroup(EditorTextEditorGroupKey);

        var inputFileAbsoluteFilePathString = absoluteFilePath.GetAbsoluteFilePathString();

        var textEditorBase = textEditorService
            .GetTextEditorBaseOrDefaultByResourceUri(inputFileAbsoluteFilePathString);

        var textEditorKey = textEditorBase?.Key ?? TextEditorKey.Empty;
        
        if (textEditorBase is null)
        {
            textEditorKey = TextEditorKey.NewTextEditorKey();
            
            var content = await File
                .ReadAllTextAsync(inputFileAbsoluteFilePathString);

            textEditorBase = new TextEditorBase(
                inputFileAbsoluteFilePathString,
                absoluteFilePath.ExtensionNoPeriod,
                content,
                ExtensionNoPeriodFacts.GetLexer(absoluteFilePath.ExtensionNoPeriod),
                ExtensionNoPeriodFacts.GetDecorationMapper(absoluteFilePath.ExtensionNoPeriod),
                null,
                textEditorKey
            );
            
            textEditorService.RegisterCustomTextEditor(textEditorBase);

            textEditorKey = textEditorBase.Key;
            
            await textEditorBase.ApplySyntaxHighlightingAsync();
        }

        var viewModel = textEditorService
            .GetViewModelsForTextEditorBase(textEditorBase.Key)
            .FirstOrDefault();

        var viewModelKey = viewModel?.TextEditorViewModelKey ?? TextEditorViewModelKey.Empty;

        if (viewModel is null)
        {
            viewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
            
            textEditorService.RegisterViewModel(
                viewModelKey,
                textEditorKey);
            
            textEditorService.SetViewModelWith(
                viewModelKey,
                textEditorViewModel => textEditorViewModel with
                {
                    OnSaveRequested = HandleOnSaveRequested,
                    GetTabDisplayNameFunc = _ => absoluteFilePath.FilenameWithExtension
                });
        }
            
        textEditorService.AddViewModelToGroup(
            EditorTextEditorGroupKey,
            viewModelKey);
        
        textEditorService.SetActiveViewModelOfGroup(
            EditorTextEditorGroupKey,
            viewModelKey);

        void HandleOnSaveRequested(TextEditorBase innerTextEditor)
        {
            var innerContent = innerTextEditor.GetAllText();
                
            var saveFileAction = new FileSystemState.SaveFileAction(
                absoluteFilePath,
                innerContent);
        
            dispatcher.Dispatch(saveFileAction);
        
            innerTextEditor.ClearEditBlocks();
        }
    }
}