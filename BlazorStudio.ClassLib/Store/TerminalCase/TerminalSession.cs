using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalSession
{
    private readonly List<TerminalCommand> _terminalCommandsHistory = new();

    private readonly ConcurrentQueue<TerminalCommand> _terminalCommandsConcurrentQueue = new();

    /// <summary>
    /// TODO: Prove that standard error is correctly being redirected to standard out
    /// </summary>
    private readonly Dictionary<TerminalCommandKey, StringBuilder> _standardOutBuilderMap = new();

    public TerminalSession(string? workingDirectoryAbsoluteFilePathString)
    {
        WorkingDirectoryAbsoluteFilePathString = workingDirectoryAbsoluteFilePathString;
    }

    private readonly SemaphoreSlim _lifeOfTerminalCommandConsumerSemaphoreSlim = new(1, 1);
    private bool _hasTerminalCommandConsumer;
    
    public TerminalSessionKey TerminalSessionKey { get; init; } = 
        TerminalSessionKey.NewTerminalSessionKey();

    public string? WorkingDirectoryAbsoluteFilePathString { get; private set; }
    
    public IDispatcher Dispatcher { get; }
    
    public TerminalCommand ActiveTerminalCommand { get; private set; }
    
    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();

    public string? ReadStandardOut()
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
        // Task.Run as to not have a chance of blocking the UI thread?
        _ = Task.Run(async () =>
        {
            await ConsumeTerminalCommandsAsync();
        });
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
        
        var process = new Process();

        if (WorkingDirectoryAbsoluteFilePathString is not null)
        {
            process.StartInfo.WorkingDirectory = 
                WorkingDirectoryAbsoluteFilePathString;   
        }

        var bash = "/bin/bash";

        if (File.Exists(bash))
        {
            process.StartInfo.FileName = bash;
            process.StartInfo.Arguments = $"-c \"{terminalCommand.Command}\"";
        }
        else
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {terminalCommand.Command} 2>&1";
        }

        // Start the child process.
        // 2>&1 combines stdout and stderr
        //process.StartInfo.Arguments = $"";
        // Redirect the output stream of the child process.
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;

        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _standardOutBuilderMap[terminalCommand.TerminalCommandKey]
                .Append(e.Data ?? string.Empty);
        }

        process.OutputDataReceived += OutputDataReceived;
        
        try
        {
            _standardOutBuilderMap.TryAdd(
                terminalCommand.TerminalCommandKey,
                new StringBuilder());
            
            process.Start();

            process.BeginOutputReadLine();
            
            await process.WaitForExitAsync();
        }
        finally
        {
            process.CancelOutputRead();
        }

        goto doConsumeLabel;
    }
}