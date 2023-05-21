using System.Collections.Immutable;
using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.ComponentRenderers.Types;
using BlazorCommon.RazorLib.Misc;
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
using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Semantics;
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
        IFileSystemProvider fileSystemProvider,
        IBackgroundTaskQueue backgroundTaskQueue)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "TextEditor",
                async afp =>
                {
                    await OpenInEditorAsync(
                        afp,
                        true,
                        dispatcher, 
                        textEditorService,
                        blazorStudioComponentRenderers,
                        fileSystemProvider,
                        backgroundTaskQueue);
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
        bool shouldSetFocusToEditor,
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IBackgroundTaskQueue backgroundTaskQueue)
    {
        if (absoluteFilePath is null ||
            absoluteFilePath.IsDirectory)
        {
            return;
        }
        
        textEditorService.Group.Register(EditorTextEditorGroupKey);

        var inputFileAbsoluteFilePathString = absoluteFilePath.GetAbsoluteFilePathString();

        var textEditorModel = await GetOrCreateTextEditorModelAsync(
            textEditorService,
            fileSystemProvider,
            absoluteFilePath,
            inputFileAbsoluteFilePathString);
        
        await CheckIfContentsWereModifiedAsync(
            dispatcher,
            textEditorService,
            blazorStudioComponentRenderers,
            fileSystemProvider,
            backgroundTaskQueue,
            inputFileAbsoluteFilePathString,
            textEditorModel);

        var viewModel = GetOrCreateTextEditorViewModel(
            absoluteFilePath,
            shouldSetFocusToEditor,
            dispatcher,
            textEditorService,
            fileSystemProvider,
            backgroundTaskQueue,
            textEditorModel,
            inputFileAbsoluteFilePathString);

        textEditorService.Group.AddViewModel(
            EditorTextEditorGroupKey,
            viewModel);
        
        textEditorService.Group.SetActiveViewModel(
            EditorTextEditorGroupKey,
            viewModel);
    }

    private static async Task<TextEditorModel> GetOrCreateTextEditorModelAsync(
        ITextEditorService textEditorService,
        IFileSystemProvider fileSystemProvider,
        IAbsoluteFilePath absoluteFilePath,
        string inputFileAbsoluteFilePathString)
    {
        var textEditorModel = textEditorService.Model
            .FindOrDefaultByResourceUri(inputFileAbsoluteFilePathString);
        
        if (textEditorModel is null)
        {
            var fileLastWriteTime = await fileSystemProvider.File.GetLastWriteTimeAsync(
                inputFileAbsoluteFilePathString);
            
            var content = await fileSystemProvider.File.ReadAllTextAsync(
                inputFileAbsoluteFilePathString);

            var lexer = ExtensionNoPeriodFacts.GetLexer(
                absoluteFilePath.ExtensionNoPeriod);

            var decorationMapper = ExtensionNoPeriodFacts.GetDecorationMapper(
                absoluteFilePath.ExtensionNoPeriod);

            var semanticModel = ExtensionNoPeriodFacts.GetSemanticModel(
                absoluteFilePath.ExtensionNoPeriod,
                lexer);

            textEditorModel = new TextEditorModel(
                inputFileAbsoluteFilePathString,
                fileLastWriteTime,
                absoluteFilePath.ExtensionNoPeriod,
                content,
                lexer,
                decorationMapper,
                semanticModel,
                null,
                TextEditorModelKey.NewTextEditorModelKey()
            );
            
            textEditorService.Model.RegisterCustom(textEditorModel);

            _ = Task.Run(async () =>
                await textEditorModel.ApplySyntaxHighlightingAsync());
        }

        return textEditorModel;
    }

    private static async Task CheckIfContentsWereModifiedAsync(
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IBackgroundTaskQueue backgroundTaskQueue,
        string inputFileAbsoluteFilePathString,
        TextEditorModel textEditorModel)
    {
        var fileLastWriteTime = await fileSystemProvider.File.GetLastWriteTimeAsync(
            inputFileAbsoluteFilePathString);

        if (fileLastWriteTime > textEditorModel.ResourceLastWriteTime &&
            blazorStudioComponentRenderers.BooleanPromptOrCancelRendererType is not null)
        {
            var notificationInformativeKey = NotificationKey.NewNotificationKey();

            var notificationInformative = new NotificationRecord(
                notificationInformativeKey,
                "File contents were modified on disk",
                blazorStudioComponentRenderers.BooleanPromptOrCancelRendererType,
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
                            var backgroundTask = new BackgroundTask(
                                async cancellationToken =>
                                {
                                    dispatcher.Dispatch(
                                        new NotificationRecordsCollection.DisposeAction(
                                            notificationInformativeKey));

                                    var content = await fileSystemProvider.File
                                        .ReadAllTextAsync(inputFileAbsoluteFilePathString);

                                    textEditorService.Model.Reload(
                                        textEditorModel.ModelKey,
                                        content,
                                        fileLastWriteTime);

                                    await textEditorModel.ApplySyntaxHighlightingAsync();
                                },
                                "FileContentsWereModifiedOnDiskTask",
                                "TODO: Describe this task",
                                false,
                                _ => Task.CompletedTask,
                                dispatcher,
                                CancellationToken.None);

                            backgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
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
    
    private static TextEditorViewModelKey GetOrCreateTextEditorViewModel(
        IAbsoluteFilePath absoluteFilePath,
        bool shouldSetFocusToEditor,
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        IFileSystemProvider fileSystemProvider,
        IBackgroundTaskQueue backgroundTaskQueue,
        TextEditorModel textEditorModel,
        string? inputFileAbsoluteFilePathString)
    {
        var viewModel = textEditorService.Model
            .GetViewModelsOrEmpty(textEditorModel.ModelKey)
            .FirstOrDefault();

        var viewModelKey = viewModel?.ViewModelKey ?? TextEditorViewModelKey.Empty;

        if (viewModel is null)
        {
            viewModelKey = TextEditorViewModelKey.NewTextEditorViewModelKey();

            textEditorService.ViewModel.Register(
                viewModelKey,
                textEditorModel.ModelKey);

            textEditorService.ViewModel.With(
                viewModelKey,
                textEditorViewModel => textEditorViewModel with
                {
                    OnSaveRequested = HandleOnSaveRequested,
                    GetTabDisplayNameFunc = _ => absoluteFilePath.FilenameWithExtension,
                    ShouldSetFocusAfterNextRender = shouldSetFocusToEditor
                });

            //viewModel = textEditorService.ViewModel.FindOrDefault(viewModelKey);

            //if (viewModel is not null)
            //{
            //    ChangeLastPresentationLayer(
            //        textEditorService,
            //        viewModel);
            //}
        }
        else
        {
            viewModel.ShouldSetFocusAfterNextRender = shouldSetFocusToEditor;
        }

        return viewModelKey;
        
        void HandleOnSaveRequested(TextEditorModel innerTextEditor)
        {
            var innerContent = innerTextEditor.GetAllText();

            var saveFileAction = new FileSystemState.SaveFileAction(
                absoluteFilePath,
                innerContent,
                () =>
                {
                    var backgroundTask = new BackgroundTask(
                        async cancellationToken =>
                        {
                            var fileLastWriteTime = await fileSystemProvider.File
                                .GetLastWriteTimeAsync(
                                    inputFileAbsoluteFilePathString,
                                    cancellationToken);

                            textEditorService.Model.SetResourceData(
                                textEditorModel.ModelKey,
                                textEditorModel.ResourceUri,
                                fileLastWriteTime);
                        },
                        "HandleOnSaveRequestedTask",
                        "TODO: Describe this task",
                        false,
                        _ => Task.CompletedTask,
                        dispatcher,
                        CancellationToken.None);

                    backgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
                });

            dispatcher.Dispatch(saveFileAction);

            innerTextEditor.ClearEditBlocks();
        }
    }

    //private static void ChangeLastPresentationLayer(
    //    ITextEditorService textEditorService,
    //    TextEditorViewModel viewModel)
    //{
    //    textEditorService.ViewModel.With(
    //        viewModel.ViewModelKey,
    //        inViewModel =>
    //        {
    //            var outPresentationLayer = inViewModel.FirstPresentationLayer;

    //            var inPresentationModel = outPresentationLayer
    //                .FirstOrDefault(x =>
    //                    x.TextEditorPresentationKey == SemanticFacts.PresentationKey);

    //            if (inPresentationModel is null)
    //            {
    //                inPresentationModel = SemanticFacts.EmptyPresentationModel;

    //                outPresentationLayer = outPresentationLayer.Add(
    //                    inPresentationModel);
    //            }

    //            var model = textEditorService.ViewModel
    //                .FindBackingModelOrDefault(viewModel.ViewModelKey);

    //            var outPresentationModel = inPresentationModel with
    //            {
    //                TextEditorTextSpans = model?.SemanticModel?.TextEditorTextSpans
    //                    ?? ImmutableList<TextEditorTextSpan>.Empty
    //            };

    //            outPresentationLayer = outPresentationLayer.Replace(
    //                inPresentationModel,
    //                outPresentationModel);

    //            return inViewModel with
    //            {
    //                FirstPresentationLayer = outPresentationLayer,
    //                RenderStateKey = RenderStateKey.NewRenderStateKey()
    //            };
    //        });
    //}
}