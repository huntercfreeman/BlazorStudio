using System.Diagnostics;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalSession
{
    private TerminalCommand _terminalCommand;

    private TerminalSession(TerminalCommand terminalCommand)
    {
        _terminalCommand = terminalCommand;
    }

    public static async Task<TerminalSession> BeginSession(TerminalCommand terminalCommand)
    {
        return new TerminalSession(terminalCommand);
    }
    
    public async Task<TerminalSession> ExecuteCommand(
        string command,
        IDispatcher dispatcher)
    {
        var process = new Process();

        if (_terminalCommand.WorkingDirectoryAbsoluteFilePathString is not null)
        {
            process.StartInfo.WorkingDirectory = 
                _terminalCommand.WorkingDirectoryAbsoluteFilePathString;   
        }

        var bash = "/bin/bash";

        if (File.Exists(bash))
        {
            process.StartInfo.FileName = bash;
            process.StartInfo.Arguments = $"-c \"{command}\"";
        }
        else
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command} 2>&1";
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
            // TODO: Remove this temporary hack to get the UI to rerender am tired and needed a motivation boost from seeing the progress visually
            _terminalCommand = _terminalCommand with
            {
                
            };
            
            _terminalCommand.StandardOut
                .Append(e.Data ?? string.Empty);
            
            dispatcher.Dispatch(
                new ReplaceTerminalResultAction(
                    _terminalCommand));
        }

        process.OutputDataReceived += OutputDataReceived;
        
        try
        {
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