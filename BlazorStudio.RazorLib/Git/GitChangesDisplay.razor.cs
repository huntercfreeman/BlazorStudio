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

        Dispatcher.Dispatch(
            new GitState.SetGitFilesListAction(
                ImmutableList<GitFile>.Empty));

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

                await GetGitOutputSectionAsync(
                    gitState,
                    gitStatusOutput,
                    GitFacts.UNTRACKED_FILES_TEXT_START,
                    GitDirtyReason.Untracked,
                    gitFiles => Dispatcher.Dispatch(
                        new GitState.AddGitFilesAction(
                            gitFiles)));
                
                await GetGitOutputSectionAsync(
                    gitState,
                    gitStatusOutput,
                    GitFacts.CHANGES_NOT_STAGED_FOR_COMMIT_TEXT_START,
                    null,
                    gitFiles => Dispatcher.Dispatch(
                        new GitState.AddGitFilesAction(
                            gitFiles)));
            });
        
        await generalTerminalSession
            .EnqueueCommandAsync(gitStatusCommand);
    }
    
    private async Task GetGitOutputSectionAsync(
        GitState gitState,
        string gitStatusOutput,
        string sectionStart,
        GitDirtyReason? gitDirtyReason,
        Action<ImmutableList<GitFile>> onAfterCompletedAction)
    {
        if (gitState.GitFolderAbsoluteFilePath?.ParentDirectory is null)
            return;
        
        var indexOfChangesNotStagedForCommitTextStart = gitStatusOutput.IndexOf(
            sectionStart,
            StringComparison.InvariantCulture);

        if (indexOfChangesNotStagedForCommitTextStart != -1)
        {
            var startOfChangesNotStagedForCommitIndex = indexOfChangesNotStagedForCommitTextStart +
                                                        sectionStart.Length;

            var gitStatusOutputReader = new StringReader(
                gitStatusOutput.Substring(startOfChangesNotStagedForCommitIndex));

            var changesNotStagedForCommitBuilder = new StringBuilder();

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
                 * Changes not staged for commit:
                 *   (use "git add/rm <file>..." to update what will be committed)
                 *   (use "git restore <file>..." to discard changes in working directory)
                 *       modified:   BlazorCrudApp.ServerSide/Pages/Counter.razor
                 *       deleted:    BlazorCrudApp.ServerSide/Shared/SurveyPrompt.razor
                 */
                if (currentLine.StartsWith(new string(' ', 2)))
                    continue;
                
                changesNotStagedForCommitBuilder.Append(currentLine);
            }

            var changesNotStagedForCommitText = changesNotStagedForCommitBuilder.ToString();
            
            var changesNotStagedForCommitCollection = changesNotStagedForCommitText
                .Split('\t')
                .Select(x => x.Trim())
                .OrderBy(x => x)
                .ToArray();
            
            if (changesNotStagedForCommitCollection.First() == string.Empty)
            {
                changesNotStagedForCommitCollection = changesNotStagedForCommitCollection
                    .Skip(1)
                    .ToArray();
            }

            (string relativePath, GitDirtyReason gitDirtyReason)[] changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples;

            if (gitDirtyReason is not null)
            {
                changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples = changesNotStagedForCommitCollection
                    .Select(x => (x, gitDirtyReason.Value))
                    .ToArray();
            }
            else
            {
                changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples = changesNotStagedForCommitCollection
                    .Select(x =>
                    {
                        var relativePath = x;
                        GitDirtyReason innerGitDirtyReason = GitDirtyReason.None;
                        
                        if (x.StartsWith(GitFacts.GIT_DIRTY_REASON_MODIFIED))
                        {
                            innerGitDirtyReason = GitDirtyReason.Modified;
                            
                            relativePath = new string(relativePath
                                .Skip(GitFacts.GIT_DIRTY_REASON_MODIFIED.Length)
                                .ToArray());
                        }
                        else if (x.StartsWith(GitFacts.GIT_DIRTY_REASON_DELETED))
                        {
                            innerGitDirtyReason = GitDirtyReason.Deleted;
                            
                            relativePath = new string(relativePath
                                .Skip(GitFacts.GIT_DIRTY_REASON_DELETED.Length)
                                .ToArray());
                        }
                        
                        return (relativePath, innerGitDirtyReason);
                    })
                    .ToArray();
            }

            var changesNotStagedForCommitGitFiles = changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples
                .Select(x =>
                {
                    var absoluteFilePathString =
                        gitState.GitFolderAbsoluteFilePath.ParentDirectory.GetAbsoluteFilePathString() +
                        x.relativePath;
                    
                    var isDirectory = x.relativePath.EndsWith(Path.DirectorySeparatorChar) ||
                                      x.relativePath.EndsWith(Path.AltDirectorySeparatorChar);
                    
                    var absoluteFilePath = new AbsoluteFilePath(
                        absoluteFilePathString,
                        isDirectory);

                    return new GitFile(absoluteFilePath, x.gitDirtyReason);
                })
                .ToImmutableList();

            onAfterCompletedAction.Invoke(changesNotStagedForCommitGitFiles);
        }
    }
}