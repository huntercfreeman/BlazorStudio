using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalDisplay : FluxorComponent, IDisposable
{
    [Inject]
    private IState<TerminalState> TerminalStatesWrap { get; set; } = null!;
    [Inject]
    private IState<WorkspaceState> WorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    private string _commandLineInput = string.Empty;
    private string _commandLineOutput = string.Empty;
    private StringBuilder _processOnOutputDataReceived = new();
    private bool _isExecuting;
    private Process _process;
    private int _port;
    private string _getProcessIdRunningOnPortOutput;
    private int _killedProcessesCounter;
    private int _processId;

    protected override void OnInitialized()
    {
        WorkspaceStateWrap.StateChanged += WorkspaceStateWrapOnStateChanged;
        
        _process = new();

        _process.OutputDataReceived += ProcessOnOutputDataReceived;
        _process.ErrorDataReceived += ProcessOnOutputDataReceived;

        base.OnInitialized();
    }

    private async void WorkspaceStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        _process.StartInfo.WorkingDirectory =
            WorkspaceStateWrap.Value.WorkspaceAbsoluteFilePath?.GetAbsoluteFilePathString();

        await InvokeAsync(StateHasChanged);
    }

    private async void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        var indexOfHttp = e.Data.IndexOf("http");

        if (indexOfHttp > 0)
        {
            var firstSubstring = e.Data.Substring(0, indexOfHttp);
            
            var httpBuilder = new StringBuilder();

            var position = indexOfHttp;

            while (position < e.Data.Length)
            {
                var currentCharacter = e.Data[position++];

                if (currentCharacter == ' ')
                {
                    break;
                }
                else
                {
                    httpBuilder.Append(currentCharacter);
                }
            }

            var aTag = $"<a href=\"{httpBuilder}\">{httpBuilder}</a>";

            var result = firstSubstring.EscapeHtml()
                         + aTag.ToString();

            if (position != e.Data.Length - 1)
            {
                result += e.Data.Substring(position);
            }

            _processOnOutputDataReceived.Append(result + "<br />");
        }
        else
        {
            _processOnOutputDataReceived.Append(e.Data.EscapeHtml() + "<br />");
        }


        await InvokeAsync(StateHasChanged);
    }

    private async Task OnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.NewLineCodes.ENTER_CODE)
        {
            await Task.Run(async () =>
            {
                // Start the child process.
                _process.StartInfo.FileName = "cmd.exe";
                // 2>&1 combines stdout and stderr
                _process.StartInfo.Arguments = $"/c {_commandLineInput} 2>&1";
                // Redirect the output stream of the child process.
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.RedirectStandardOutput = true;
                _process.StartInfo.CreateNoWindow = true;

                try
                {
                    _isExecuting = true;
                    await InvokeAsync(StateHasChanged);

                    _process.Start();
                    
                    _process.BeginOutputReadLine();

                    await _process.WaitForExitAsync();
                }
                finally
                {
                    _isExecuting = false;
                    _process.CancelOutputRead();
                    await InvokeAsync(StateHasChanged);
                }
            });
        }
    }

    private async Task GetProcessIdRunningOnPort(int port)
    {
        var command = $"netstat -ano | findStr \"{port}\"";
        var z = "tasklist /fi \"pid eq 2216\"";

        await Task.Run(async () =>
        {
            var p = new Process();
            // Start the child process.
            p.StartInfo.FileName = "cmd.exe";
            // 2>&1 combines stdout and stderr
            p.StartInfo.Arguments = $"/c {command} 2>&1";
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            try
            {
                await InvokeAsync(StateHasChanged);

                p.Start();

                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                _getProcessIdRunningOnPortOutput = p.StandardOutput.ReadToEnd();

                await p.WaitForExitAsync();
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        });
    }
    
    private async Task KillProcessWithProcessId(int processId)
    {
        if (processId == 0)
        {
            return;
        }

        var command = $"taskkill /PID {processId} /F";

        await Task.Run(async () =>
        {
            var p = new Process();
            // Start the child process.
            p.StartInfo.FileName = "cmd.exe";
            // 2>&1 combines stdout and stderr
            p.StartInfo.Arguments = $"/c {command} 2>&1";
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            try
            {
                await InvokeAsync(StateHasChanged);

                p.Start();
                
                await p.WaitForExitAsync();
            }
            finally
            {
                _killedProcessesCounter++;
                await InvokeAsync(StateHasChanged);
            }
        });

    }

    protected override void Dispose(bool disposing)
    {
        WorkspaceStateWrap.StateChanged -= WorkspaceStateWrapOnStateChanged;
        _process.OutputDataReceived -= ProcessOnOutputDataReceived;

        _process.Kill();
        _process.Dispose();

        base.Dispose(disposing);
    }
}