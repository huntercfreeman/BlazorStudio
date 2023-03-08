using System.Collections.Concurrent;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.NotificationCase;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.FileSystemCase;

public class FileSystemState
{
    public const int MAXIMUM_CHARACTER_COUNT_OF_CONTENT = 250;
    
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    
    /// <summary>
    /// "string: absoluteFilePath"
    /// <br/>
    /// "ValueTuple: containing the parameters to <see cref="PerformWriteOperationAsync"/>"
    /// </summary>
    private readonly ConcurrentDictionary<
        string, 
        (string absoluteFilePathString, SaveFileAction saveFileAction, IDispatcher dispatcher)?>
        _concurrentMapToTasksForThrottleByFile = new();
    
    public FileSystemState(
        IFileSystemProvider fileSystemProvider,
        ICommonComponentRenderers commonComponentRenderers)
    {
        _fileSystemProvider = fileSystemProvider;
        _commonComponentRenderers = commonComponentRenderers;
    }
    
    public record SaveFileAction(
        IAbsoluteFilePath AbsoluteFilePath,
        string Content,
        Action OnAfterSaveCompleted);
    
    [EffectMethod]
    public Task HandleSaveFileAction(
        SaveFileAction saveFileAction,
        IDispatcher dispatcher)
    {
        var absoluteFilePathString = saveFileAction.AbsoluteFilePath
            .GetAbsoluteFilePathString();

        void FireAndForgetConsumerFirstLoop()
        {
            // The first loop relies on the downstream code 'bool isFirstLoop = true;'
            Task.Run(async () =>
                await PerformWriteOperationAsync(
                    absoluteFilePathString, 
                    saveFileAction, 
                    dispatcher));
        }

        // Produce write task and construct consumer if necessary
        _ = _concurrentMapToTasksForThrottleByFile
            .AddOrUpdate(absoluteFilePathString,
                absoluteFilePath =>
                {
                    FireAndForgetConsumerFirstLoop();
                    return null;
                },
                (absoluteFilePath, foundExistingValue) =>
                {
                    if (foundExistingValue is null)
                    {
                        FireAndForgetConsumerFirstLoop();
                        return null;
                    }
                    
                    return (absoluteFilePathString, saveFileAction, dispatcher);
                });
        return Task.CompletedTask;
    }

    private async Task PerformWriteOperationAsync(
        string absoluteFilePathString, 
        SaveFileAction saveFileAction,
        IDispatcher dispatcher)
    {
        bool isFirstLoop = true;
        
        // goto is used because the do-while or while loops would have
        // hard to decipher predicates due to the double if for the semaphore
        doConsumeLabel:

        (string absoluteFilePathString, 
            SaveFileAction saveFileAction, 
            IDispatcher dispatcher)? 
            writeRequest;
        
        if (isFirstLoop)
        {
            // Perform the first request
            writeRequest = (absoluteFilePathString, saveFileAction, dispatcher);
        }
        else
        {
            // Take most recent write request.
            //
            // Then update most recent write request to be
            // null as to throttle and take the most recent and
            // discard the in between events.
            writeRequest = _concurrentMapToTasksForThrottleByFile
                .AddOrUpdate(absoluteFilePathString,
                    absoluteFilePath =>
                    {
                        // This should never occur as 
                        // being in this method is dependent on
                        // a value having already existed
                        return null;
                    },
                    (absoluteFilePath, foundExistingValue) =>
                    {
                        if (foundExistingValue is null)
                            return null;

                        return foundExistingValue;
                    });
        }

        if (writeRequest is null)
            return;

        isFirstLoop = false;
        
        string notificationMessage;
        
        if (absoluteFilePathString is not null &&
            await _fileSystemProvider.File.ExistsAsync(absoluteFilePathString))
        {
            await _fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePathString,
                saveFileAction.Content);
        
            if (saveFileAction.Content.Length > MAXIMUM_CHARACTER_COUNT_OF_CONTENT)
            {
                notificationMessage = $"{MAXIMUM_CHARACTER_COUNT_OF_CONTENT} character limit was reached. Partially saved: {absoluteFilePathString}";
            }
            else
            {
                notificationMessage = $"successfully saved: {absoluteFilePathString}";
            }
        }
        else
        {
            // TODO: Save As to make new file
            notificationMessage = "File not found. TODO: Save As";
        }
        
        var notificationInformative  = new NotificationRecord(
            NotificationKey.NewNotificationKey(), 
            "Save Action",
            _commonComponentRenderers.InformativeNotificationRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(IInformativeNotificationRendererType.Message), 
                    notificationMessage
                },
            },
            TimeSpan.FromSeconds(5));
        
        dispatcher.Dispatch(
            new NotificationRecordsCollection.RegisterAction(
                notificationInformative));
        
        saveFileAction.OnAfterSaveCompleted?.Invoke();

        goto doConsumeLabel;
    }
}