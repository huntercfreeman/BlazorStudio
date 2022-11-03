using System.Collections.Concurrent;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using Fluxor;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.FileSystemCase;

public class FileSystemState
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ICommonComponentRenderers _commonComponentRenderers;

    /// <summary>
    /// One write task can be made at a time
    /// <br/><br/>
    /// Many write operations can be processed concurrently however,
    /// write operations to the SAME file are limited until the previous
    /// write to that file is finished.
    /// </summary>
    private readonly SemaphoreSlim _writeFileSemaphoreSlim = new(1, 1);
    /// <summary>
    /// Maintain a Dictionary to map between a "string: absoluteFilePath" and
    /// the corresponding Task that is a series of ContinueWith() operations.
    /// <br/><br/>
    /// TODO: Instead of doing ContinueWith() use a throttle of which
    /// the most recent "save file" request is processed and the rest are discarded.
    /// </summary>
    private readonly Dictionary<string, Task> _writeTaskMap = new();

    /// <summary>
    /// "string: absoluteFilePath", "Task: writeTask"
    /// </summary>
    private readonly ConcurrentDictionary<
        string, 
        (string absoluteFilePathString, SaveFileAction saveFileAction, IDispatcher dispatcher)?>
        _concurrentMapToTasksForThrottleByFile = new();
    
    private readonly ConcurrentDictionary<
        string, 
        (bool hasConsumer, SemaphoreSlim consumerLifeSemaphoreSlim)> 
        _concurrentMapToConsumerTuplesForThrottleByFile = new();
    
    public FileSystemState(
        IFileSystemProvider fileSystemProvider,
        ICommonComponentRenderers commonComponentRenderers)
    {
        _fileSystemProvider = fileSystemProvider;
        _commonComponentRenderers = commonComponentRenderers;
    }
    
    public record SaveFileAction(IAbsoluteFilePath AbsoluteFilePath, string Content);
    
    [EffectMethod]
    public async Task HandleSaveFileAction(
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
        
        string notificationInformativeMessage;
        
        if (File.Exists(absoluteFilePathString))
        {
            await File.WriteAllTextAsync(
                absoluteFilePathString,
                saveFileAction.Content);
        
            notificationInformativeMessage = $"successfully saved: {absoluteFilePathString}";
        }
        else
        {
            // TODO: Save As to make new file
            notificationInformativeMessage = "File not found. TODO: Save As";
        }
        
        var notificationInformative  = new NotificationRecord(
            NotificationKey.NewNotificationKey(), 
            "Save Action",
            _commonComponentRenderers.InformativeNotificationRenderer,
            new Dictionary<string, object?>
            {
                {
                    // TODO: make constant for "Message"
                    "Message", 
                    notificationInformativeMessage
                },
            });
        
        dispatcher.Dispatch(
            new NotificationState.RegisterNotificationAction(
                notificationInformative));

        goto doConsumeLabel;
    }
}