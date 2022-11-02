using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public partial record TerminalSession
{
    public TerminalSessionKey TerminalSessionKey { get; init; } = 
        TerminalSessionKey.NewTerminalSessionKey();

    public string? WorkingDirectoryAbsoluteFilePathString { get; init; }

    public ImmutableList<TerminalCommand> TerminalCommandsHistory { get; init; }
    public TerminalCommandKey CurrentlyExecutingTerminalCommandKey { get; init; }

    /// <summary>
    /// TODO: Prove that standard error is correctly being redirected to standard out
    /// </summary>
    private Dictionary<TerminalCommandKey, StringBuilder> _standardOutBuilderMap { get; } = new();

    public string ReadStandardOut()
    {
        return string
            .Join(string.Empty, _standardOutBuilderMap
                .Select(x => x.Value.ToString())
                .ToArray());
    }
    
    public string ReadStandardOut(TerminalCommandKey specificTerminalCommandKey)
    {
        return _standardOutBuilderMap[specificTerminalCommandKey]
            .ToString();
    }
    
    public async Task<TerminalSession> ExecuteCommand(
        TerminalCommand terminalCommand,
        IDispatcher dispatcher)
    {
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

        return this;
    }
}