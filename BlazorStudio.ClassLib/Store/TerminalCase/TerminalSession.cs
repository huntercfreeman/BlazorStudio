using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.State;
using CliWrap;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalSession
{
    private readonly IDispatcher _dispatcher;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly List<TerminalCommand> _terminalCommandsHistory = new();
    private readonly SemaphoreSlim _lifeOfTerminalCommandConsumerSemaphoreSlim = new(1, 1);
    private bool _hasTerminalCommandConsumer;
    private CancellationTokenSource _commandCancellationTokenSource = new();

    private readonly ConcurrentQueue<TerminalCommand> _terminalCommandsConcurrentQueue = new();

    /// <summary>
    /// TODO: Prove that standard error is correctly being redirected to standard out
    /// </summary>
    private readonly Dictionary<TerminalCommandKey, StringBuilder> _standardOutBuilderMap = new();

    public TerminalSession(
        string? workingDirectoryAbsoluteFilePathString, 
        IDispatcher dispatcher,
        IFileSystemProvider fileSystemProvider,
        IBackgroundTaskQueue backgroundTaskQueue)
    {
        _dispatcher = dispatcher;
        _fileSystemProvider = fileSystemProvider;
        _backgroundTaskQueue = backgroundTaskQueue;
        WorkingDirectoryAbsoluteFilePathString = workingDirectoryAbsoluteFilePathString;
    }

    public TerminalSessionKey TerminalSessionKey { get; init; } = 
        TerminalSessionKey.NewTerminalSessionKey();

    public string? WorkingDirectoryAbsoluteFilePathString { get; private set; }
    
    public TerminalCommand? ActiveTerminalCommand { get; private set; }
    
    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();
    
    // NOTE: the following did not work => _process?.HasExited ?? false;
    public bool HasExecutingProcess { get; private set; }
    
    public string ReadStandardOut()
    {
        return string
            .Join(string.Empty, _standardOutBuilderMap
                .Select(x => x.Value.ToString())
                .ToArray());
    }
    
    public string? ReadStandardOut(TerminalCommandKey terminalCommandKey)
    {
        if (_standardOutBuilderMap
            .TryGetValue(terminalCommandKey, out var output))
        {
            return output.ToString();
        }

        return null;
    }

    public async Task EnqueueCommandAsync(TerminalCommand terminalCommand)
    {
        _terminalCommandsConcurrentQueue.Enqueue(terminalCommand);

        try
        {
            await _lifeOfTerminalCommandConsumerSemaphoreSlim.WaitAsync();

            if (_hasTerminalCommandConsumer)
            {
                // Only 1 terminal command can run at a
                // time foreach terminal session
                //
                // thereby only have 1 consumer
                return;
            }
            else
            {
                _hasTerminalCommandConsumer = true;
            }
        }
        finally
        {
            // Task.Run was moved to after this Release otherwise deadlock
            _lifeOfTerminalCommandConsumerSemaphoreSlim.Release();
        }
        
        // If a terminal is not executing a command
        // it will 'dispose' of the consumer
        //
        // thereby a consumer will need to be
        // made if there isn't one
        //
        // BackgroundTask as to not have a chance of blocking the UI thread?
        var backgroundTask = new BackgroundTask(
            async cancellationToken =>
            {
                await ConsumeTerminalCommandsAsync();
            },
            "EnqueueCommandAsyncTask",
            "TODO: Describe this task",
            false,
            _ =>  Task.CompletedTask,
            _dispatcher,
            CancellationToken.None);

        _backgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
    }
    
    public void ClearStandardOut()
    {
        // TODO: Rewrite this - see contiguous comment block
        //
        // This is awkward but concurrency exceptions I believe might occur
        // otherwise given the current way the code is written (2022-02-11)
        //
        // If one tries to write to standard out dictionary they need a key value entry
        // to exist or they add one
        // 
        // If one sees a key value entry exists they can use the existing StringBuilder
        // but I am tempted to write _standardOutBuilderMap.Clear() thereby
        // clearing all the key value pairs as they write to the StringBuilder.
        foreach (var stringBuilder in _standardOutBuilderMap.Values)
        {
            stringBuilder.Clear();
        }
    }
    
    public void KillProcess()
    {
        _commandCancellationTokenSource.Cancel();
        _commandCancellationTokenSource = new();
    }
    
    private async Task ConsumeTerminalCommandsAsync()
    {
        // goto is used because the do-while or while loops would have
        // hard to decipher predicates due to the double if for the semaphore
        doConsumeLabel:
        
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

        if (terminalCommand.ChangeWorkingDirectoryTo is not null)
            WorkingDirectoryAbsoluteFilePathString = terminalCommand.ChangeWorkingDirectoryTo;

        _terminalCommandsHistory.Add(terminalCommand);
        ActiveTerminalCommand = terminalCommand;

        var command = Cli.Wrap(terminalCommand.TargetFilePath);

        if (terminalCommand.Arguments.Any())
            command = command.WithArguments(terminalCommand.Arguments);

        if (terminalCommand.ChangeWorkingDirectoryTo is not null)
        {
            command = command
                .WithWorkingDirectory(terminalCommand.ChangeWorkingDirectoryTo);
        }
        else if (WorkingDirectoryAbsoluteFilePathString is not null)
        {
            command = command
                .WithWorkingDirectory(WorkingDirectoryAbsoluteFilePathString);
        }

        {
            var terminalCommandKey = terminalCommand.TerminalCommandKey;
            
            command
                .WithStandardErrorPipe(
                    PipeTarget.ToDelegate(text =>
                    {
                        _standardOutBuilderMap[terminalCommandKey]
                            .Append(text);

                        DispatchNewStateKey();
                    }))
                .WithStandardOutputPipe(PipeTarget.ToDelegate(text =>
                {
                    _standardOutBuilderMap[terminalCommandKey]
                        .Append(text);

                    DispatchNewStateKey();
                }));
        }
        
        // TODO: There was a try wrapping this command.ExecuteAsync() logic. I removed it on 2023-04-12 because it seemingly is no longer needed. It only had a finally to dispose of resources.
        _standardOutBuilderMap.TryAdd(
            terminalCommand.TerminalCommandKey,
            new StringBuilder());

        HasExecutingProcess = true;
        DispatchNewStateKey();
                
        await command.ExecuteAsync(
            _commandCancellationTokenSource.Token);
            
        HasExecutingProcess = false;
        DispatchNewStateKey();

        if (terminalCommand.ContinueWith is not null)
        {
            var continueWith = terminalCommand.ContinueWith;
            
            var backgroundTask = new BackgroundTask(
                async cancellationToken =>
                {
                    await continueWith.Invoke();
                },
                "TerminalCommand.ContinueWithTask",
                "TODO: Describe this task",
                false,
                _ =>  Task.CompletedTask,
                _dispatcher,
                CancellationToken.None);

            _backgroundTaskQueue.QueueBackgroundWorkItem(backgroundTask);
        }

        goto doConsumeLabel;
    }

    private void DispatchNewStateKey()
    {
        _dispatcher.Dispatch(
            new TerminalSessionWasModifiedStateReducer.SetTerminalSessionStateKeyAction(
                TerminalSessionKey, 
                StateKey.NewStateKey()));
    }
}