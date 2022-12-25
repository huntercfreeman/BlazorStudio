using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification.NotificationCase;
using BlazorStudio.ClassLib.CommonComponents;
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
        ITextEditorService textEditorService,
        ICommonComponentRenderers commonComponentRenderers)
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
                        commonComponentRenderers);
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
        ICommonComponentRenderers commonComponentRenderers)
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

            var fileLastWriteTime = File.GetLastWriteTime(inputFileAbsoluteFilePathString);
            
            var content = await File
                .ReadAllTextAsync(inputFileAbsoluteFilePathString);

            textEditorBase = new TextEditorBase(
                inputFileAbsoluteFilePathString,
                fileLastWriteTime,
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
        else
        {
            var fileLastWriteTime = File.GetLastWriteTime(inputFileAbsoluteFilePathString);

            if (fileLastWriteTime > textEditorBase.ResourceLastWriteTime)
            {
                var notificationInformativeKey = NotificationKey.NewNotificationKey();
                
                var notificationInformative  = new NotificationRecord(
                    notificationInformativeKey, 
                    "File contents were modified on disk",
                    commonComponentRenderers.BooleanPromptOrCancelRendererType,
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(IBooleanPromptOrCancelRendererType.Message), 
                            "File contents were modified on disk"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.AcceptOptionTextOverride), 
                            "Reload"
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterAcceptAction), 
                            new Action(() =>
                            {
                                _ = Task.Run(async () =>
                                {
                                    dispatcher.Dispatch(
                                        new NotificationsState.DisposeNotificationRecordAction(
                                            notificationInformativeKey));
                                    
                                    var content = await File
                                        .ReadAllTextAsync(inputFileAbsoluteFilePathString);
                                
                                    textEditorService.ReloadTextEditorBase(
                                        textEditorKey,
                                        content);
                                
                                    await textEditorBase.ApplySyntaxHighlightingAsync();
                                });
                            })
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineAction), 
                            new Action(() =>
                            {
                                dispatcher.Dispatch(
                                    new NotificationsState.DisposeNotificationRecordAction(
                                        notificationInformativeKey));
                            })
                        },
                    },
                    TimeSpan.FromSeconds(20));
        
                dispatcher.Dispatch(
                    new NotificationsState.RegisterNotificationRecordAction(
                        notificationInformative));
            }
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
                innerContent,
                () =>
                {
                    var fileLastWriteTime = File.GetLastWriteTime(inputFileAbsoluteFilePathString);
            
                    textEditorService.SetResourceData(
                        textEditorBase.Key,
                        textEditorBase.ResourceUri,
                        fileLastWriteTime);
                });
        
            dispatcher.Dispatch(saveFileAction);
            
            innerTextEditor.ClearEditBlocks();
        }
    }
}