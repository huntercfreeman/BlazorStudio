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
    
    private async Task GitInitOnClickAsync()
    {
        var gitState = GitStateWrap.Value;
        
        if (gitState.MostRecentTryFindGitFolderInDirectoryAction is null)
            return;
        
        var gitInitCommand = new TerminalCommand(
            GitFacts.GitInitTerminalCommandKey,
            GitCliFacts.GIT_INIT_COMMAND,
            gitState.MostRecentTryFindGitFolderInDirectoryAction.DirectoryAbsoluteFilePath.GetAbsoluteFilePathString(),
            CancellationToken.None);
        
        var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];
        
        await generalTerminalSession
            .EnqueueCommandAsync(gitInitCommand);
    }

    private async Task RefreshGitOnClickAsync()
    {
        var gitState = GitStateWrap.Value;

        if (gitState.GitFolderAbsoluteFilePath is null ||
            !Directory.Exists(gitState.GitFolderAbsoluteFilePath.GetAbsoluteFilePathString()) ||
            gitState.GitFolderAbsoluteFilePath.ParentDirectory is null)
        {
            return;
        }
        
        var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];
        
        var gitStatusCommand = new TerminalCommand(
            GitFacts.GitStatusTerminalCommandKey,
            GitCliFacts.GIT_STATUS_COMMAND,
            gitState.GitFolderAbsoluteFilePath.ParentDirectory.GetAbsoluteFilePathString(),
            CancellationToken.None,
            async () =>
            {
                var gitStatusOutput = generalTerminalSession.ReadStandardOut(
                    GitFacts.GitStatusTerminalCommandKey);

                if (gitStatusOutput is null)
                    return;

                var indexOfUntrackedFilesTextStart = gitStatusOutput.IndexOf(
                    GitFacts.UNTRACKED_FILES_TEXT_START,
                    StringComparison.InvariantCulture);

                if (indexOfUntrackedFilesTextStart != -1)
                {
                    var startOfUntrackedFilesIndex = indexOfUntrackedFilesTextStart +
                                   GitFacts.UNTRACKED_FILES_TEXT_START.Length;

                    var gitStatusOutputReader = new StringReader(
                        gitStatusOutput.Substring(startOfUntrackedFilesIndex));

                    var untrackedFilesBuilder = new StringBuilder();

                    // This skips the second newline when seeing: "\n\n"
                    string? currentLine = await gitStatusOutputReader.ReadLineAsync();
                    
                    while ((currentLine = await gitStatusOutputReader.ReadLineAsync()) is not null &&
                           currentLine.Length > 0)
                    {
                        // It is presumed that git CLI provides comments on lines
                        // which start with two space characters.
                        //
                        // Whereas output for this command starts with a tab.
                        //
                        // TODO: I imagine this is a very naive presumption and this should be revisited but I am still feeling out how to write this git logic.
                        
                        /*
                         * Untracked files:
                         *   (use "git add <file>..." to include in what will be committed)
                         *       BlazorCrudApp.ServerSide/
                         *       BlazorCrudApp.sln
                         */
                        if (currentLine.StartsWith(new string(' ', 2)))
                            continue;
                        
                        untrackedFilesBuilder.Append(currentLine);
                    }

                    var untrackedFilesText = untrackedFilesBuilder.ToString();
                    
                    var untrackedFilesRelativePaths = untrackedFilesText
                        .Split('\t');

                    if (untrackedFilesRelativePaths.First() == string.Empty)
                    {
                        untrackedFilesRelativePaths = untrackedFilesRelativePaths
                            .Skip(1)
                            .ToArray();
                    }

                    var untrackedFilesAbsolutePaths = untrackedFilesRelativePaths
                        .Select(x => new AbsoluteFilePath(
                            gitState.GitFolderAbsoluteFilePath.ParentDirectory.GetAbsoluteFilePathString() + x,
                            x.EndsWith(Path.DirectorySeparatorChar) || x.EndsWith(Path.AltDirectorySeparatorChar)))
                        .ToImmutableArray();

                    var untrackedGitFiles = untrackedFilesAbsolutePaths
                        .Select(x => new GitFile(x, GitDirtyReason.Untracked))
                        .ToImmutableList();

                    Dispatcher.Dispatch(
                        new GitState.SetGitFilesListAction(
                            untrackedGitFiles));
                }
            });
        
        await generalTerminalSession
            .EnqueueCommandAsync(gitStatusCommand);
    }
}