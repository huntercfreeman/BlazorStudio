using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Git;
using BlazorStudio.ClassLib.Store.GitCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Git;

public partial class GitChangesDisplay : FluxorComponent, IGitDisplayRendererType
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private CancellationTokenSource _gitActionCancellationTokenSource = new();
    private bool _disposedValue;
    
    private void GitInitOnClick()
    {
        Dispatcher.Dispatch(
            new GitState.GitInitAction(
                _gitActionCancellationTokenSource.Token));
    }

    private void RefreshGitOnClick()
    {
        Dispatcher.Dispatch(
            new GitState.RefreshGitAction(
                _gitActionCancellationTokenSource.Token));
    }

    private void ResetGitActionCancellationTokenSource()
    {
        _gitActionCancellationTokenSource.Cancel();
        _gitActionCancellationTokenSource = new();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _gitActionCancellationTokenSource.Cancel();
            }

            _disposedValue = true;
        }

        base.Dispose(disposing);
    }
}