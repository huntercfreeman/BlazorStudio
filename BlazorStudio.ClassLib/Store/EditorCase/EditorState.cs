using System.Collections.Immutable;
using BlazorCommon.RazorLib.ComponentRenderers.Types;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.NotificationCase;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.InputFile;
using BlazorStudio.ClassLib.Store.FileSystemCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Group;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.ViewModel;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

public class EditorState
{
    public static readonly TextEditorGroupKey EditorTextEditorGroupKey = TextEditorGroupKey.NewTextEditorGroupKey();
    
    public static Task ShowInputFileAsync(
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider)
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
                        blazorStudioComponentRenderers,
                        fileSystemProvider);
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
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider)
    {
        if (absoluteFilePath is null ||
            absoluteFilePath.IsDirectory)
        {
            return;
        }
        
        textEditorService.GroupRegister(EditorTextEditorGroupKey);

        var inputFileAbsoluteFilePathString = absoluteFilePath.GetAbsoluteFilePathString();

        var textEditorModel = textEditorService
            .ResourceUriGetModelOrDefault(inputFileAbsoluteFilePathString);

        var textEditorKey = textEditorModel?.ModelKey ?? TextEditorModelKey.Empty;
        
        if (textEditorModel is null)
        {
            textEditorKey = TextEditorModelKey.NewTextEditorModelKey();

            var fileLastWriteTime = await fileSystemProvider.File.GetLastWriteTimeAsync(
                inputFileAbsoluteFilePathString);
            
            var content = await fileSystemProvider.File.ReadAllTextAsync(
                inputFileAbsoluteFilePathString);

            textEditorModel = new TextEditorModel(
                inputFileAbsoluteFilePathString,
                fileLastWriteTime,
                absoluteFilePath.ExtensionNoPeriod,
                content,
                ExtensionNoPeriodFacts.GetLexer(absoluteFilePath.ExtensionNoPeriod),
                ExtensionNoPeriodFacts.GetDecorationMapper(absoluteFilePath.ExtensionNoPeriod),
                null,
                textEditorKey
            );
            
            textEditorService.ModelRegisterCustomModel(textEditorModel);

            textEditorKey = textEditorModel.ModelKey;
            
            await textEditorModel.ApplySyntaxHighlightingAsync();
        }
        else
        {
            var fileLastWriteTime = await fileSystemProvider.File.GetLastWriteTimeAsync(
                inputFileAbsoluteFilePathString);

            if (fileLastWriteTime > textEditorModel.ResourceLastWriteTime &&
                blazorStudioComponentRenderers.BlazorCommonComponentRenderers.BooleanPromptOrCancelRendererType is not null)
            {
                var notificationInformativeKey = NotificationKey.NewNotificationKey();
                
                var notificationInformative  = new NotificationRecord(
                    notificationInformativeKey, 
                    "File contents were modified on disk",
                    blazorStudioComponentRenderers.BlazorCommonComponentRenderers.BooleanPromptOrCancelRendererType,
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
                                        new NotificationRecordsCollection.DisposeAction(
                                            notificationInformativeKey));
                                    
                                    var content = await fileSystemProvider.File
                                        .ReadAllTextAsync(inputFileAbsoluteFilePathString);
                                
                                    textEditorService.ModelReload(
                                        textEditorKey,
                                        content);
                                
                                    await textEditorModel.ApplySyntaxHighlightingAsync();
                                });
                            })
                        },
                        {
                            nameof(IBooleanPromptOrCancelRendererType.OnAfterDeclineAction), 
                            new Action(() =>
                            {
                                dispatcher.Dispatch(
                                    new NotificationRecordsCollection.DisposeAction(
                                        notificationInformativeKey));
                            })
                        },
                    },
                    TimeSpan.FromSeconds(20),
                    null);
        
                dispatcher.Dispatch(
                    new NotificationRecordsCollection.RegisterAction(
                        notificationInformative));
            }
        }

        var viewModel = textEditorService
            .ModelGetViewModelsOrEmpty(textEditorModel.ModelKey)
            .FirstOrDefault();

        var viewModelKey = viewModel?.ViewModelKey ?? TextEditorViewModelKey.Empty;

        if (viewModel is null)
        {
            viewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();
            
            textEditorService.ViewModelRegister(
                viewModelKey,
                textEditorKey);
            
            textEditorService.ViewModelWith(
                viewModelKey,
                textEditorViewModel => textEditorViewModel with
                {
                    OnSaveRequested = HandleOnSaveRequested,
                    GetTabDisplayNameFunc = _ => absoluteFilePath.FilenameWithExtension
                });
        }
            
        textEditorService.GroupAddViewModel(
            EditorTextEditorGroupKey,
            viewModelKey);
        
        textEditorService.GroupSetActiveViewModel(
            EditorTextEditorGroupKey,
            viewModelKey);

        void HandleOnSaveRequested(TextEditorModel innerTextEditor)
        {
            var innerContent = innerTextEditor.GetAllText();
                
            var saveFileAction = new FileSystemState.SaveFileAction(
                absoluteFilePath,
                innerContent,
                () =>
                {
                    Task.Run(async () =>
                    {
                        var fileLastWriteTime = await fileSystemProvider.File
                            .GetLastWriteTimeAsync(inputFileAbsoluteFilePathString);
            
                        textEditorService.ModelSetResourceData(
                            textEditorModel.ModelKey,
                            textEditorModel.ResourceUri,
                            fileLastWriteTime);
                    });
                });
        
            dispatcher.Dispatch(saveFileAction);
            
            innerTextEditor.ClearEditBlocks();
        }
    }
}