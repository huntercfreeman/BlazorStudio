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
    private readonly ConcurrentDictionary<string, Task?> _concurrentMapToTasksForThrottleByFile = new();
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

        // Produce write task and construct consumer if necessary
        _ = _concurrentMapToTasksForThrottleByFile
            .AddOrUpdate(absoluteFilePathString,
                absoluteFilePath =>
                    Task.Run(async () => 
                        await PerformWriteOperationAsync(absoluteFilePathString, saveFileAction, dispatcher)),
                (absoluteFilePath, foundExistingValue) =>
                {
                    if (foundExistingValue is null)
                    {
                        return Task.Run(async () =>
                            await PerformWriteOperationAsync(absoluteFilePathString, saveFileAction, dispatcher));
                    }
                    
                    return PerformWriteOperationAsync(absoluteFilePathString, saveFileAction, dispatcher);
                });
    }

    private async Task PerformWriteOperationAsync(
        string absoluteFilePathString, 
        SaveFileAction saveFileAction,
        IDispatcher dispatcher)
    {
        // goto is used because the do-while or while loops would have
        // hard to decipher predicates due to the double if for the semaphore
        doConsumeLabel:

        // Take most recent write request and update it to be null
        var mostRecentWriteRequest = _concurrentMapToTasksForThrottleByFile
            .AddOrUpdate(absoluteFilePathString,
                absoluteFilePath =>
                    Task.Run(async () => 
                        await PerformWriteOperationAsync(absoluteFilePathString, saveFileAction, dispatcher)),
                (absoluteFilePath, foundExistingValue) =>
                {
                    if (foundExistingValue is null)
                    {
                        return Task.Run(async () =>
                            await PerformWriteOperationAsync(absoluteFilePathString, saveFileAction, dispatcher));
                    }
                
                    return PerformWriteOperationAsync(absoluteFilePathString, saveFileAction, dispatcher);
                });
        
        if (!_terminalCommandsConcurrentQueue.TryDequeue(out var terminalCommand))
        {
            try
            {
                await _lifeOfTerminalCommandConsumerSemaphoreSlim.WaitAsync();

                // duplicate inner if(TryDequeue) is for performance of not having to every loop
                // await the semaphore
                //
                // await semaphore only if it seems like one should dispose of the consumer
                // and then double check nothing was added in between those times
                if (!_terminalCommandsConcurrentQueue.TryDequeue(out terminalCommand))
                {
                    _hasTerminalCommandConsumer = false;
                    return;
                }   
            }
            finally
            {
                _lifeOfTerminalCommandConsumerSemaphoreSlim.Release();
            }
        }
        
        string notificationInformativeMessage;
        
        if (File.Exists(absoluteFilePathString))
        {
            try
            {
                _writeFileSemaphoreSlim
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            
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