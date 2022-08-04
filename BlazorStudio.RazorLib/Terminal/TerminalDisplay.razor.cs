using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.FileSystemApi;
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
        _processOnOutputDataReceived.AppendLine(e.Data ?? string.Empty);

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

                    // Do not wait for the child process to exit before
                    // reading to the end of its redirected stream.
                    // p.WaitForExit();
                    // Read the output stream first and then wait.

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

    protected override void Dispose(bool disposing)
    {
        WorkspaceStateWrap.StateChanged -= WorkspaceStateWrapOnStateChanged;
        _process.OutputDataReceived -= ProcessOnOutputDataReceived;

        _process.Kill();
        _process.Dispose();

        base.Dispose(disposing);
    }
}