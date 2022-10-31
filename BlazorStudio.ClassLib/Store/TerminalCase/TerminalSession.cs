using System.Diagnostics;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalSession
{
    public async Task<TerminalSession> ExecuteCommand(
        TerminalCommand terminalCommand, 
        string command)
    {
        var process = new Process();

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
            terminalCommand.StandardOut
                .Append(e.Data ?? string.Empty);
        }

        process.OutputDataReceived += OutputDataReceived;

        try
        {
            process.Start();

            process.BeginOutputReadLine();
        }
        finally
        {
            process.CancelOutputRead();
        }

        return this;
    }
}