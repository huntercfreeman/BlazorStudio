using System.Collections.Immutable;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.InputFile;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.DotNetSolutionCase;

[FeatureState]
public partial record DotNetSolutionState(
    DotNetSolution? DotNetSolution)
{
    private DotNetSolutionState() : this(default(DotNetSolution?))
    {
    }
    
    public static async Task SetActiveSolutionAsync(
        string solutionAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher)
    {
        var content = await fileSystemProvider.File.ReadAllTextAsync(
            solutionAbsolutePathString,
            CancellationToken.None);

        var solutionAbsoluteFilePath = new AbsoluteFilePath(
            solutionAbsolutePathString,
            false,
            environmentProvider);
        
        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            solutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            content,
            solutionNamespacePath,
            environmentProvider);
        
        dispatcher.Dispatch(
            new WithAction(
                inDotNetSolutionState => inDotNetSolutionState with
                {
                    DotNetSolution = dotNetSolution
                }));
    }
    
    public static Task ShowInputFileAsync(
        IDispatcher dispatcher,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "TextEditor",
                async afp =>
                {
                    await OpenInSolutionExplorerAsync(
                        afp, 
                        dispatcher,
                        blazorStudioComponentRenderers,
                        fileSystemProvider,
                        environmentProvider);
                },
                afp =>
                {
                    if (afp is null ||
                        afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(true);
                },
                new[]
                {
                    new InputFilePattern(
                        ".NET Solution",
                        afp => 
                            afp.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
                }.ToImmutableArray()));

        return Task.CompletedTask;
    }
    
    public static async Task OpenInSolutionExplorerAsync(
        IAbsoluteFilePath? absoluteFilePath,
        IDispatcher dispatcher,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        if (absoluteFilePath is null ||
            absoluteFilePath.IsDirectory)
        {
            return;
        }

        await SetActiveSolutionAsync(
            absoluteFilePath.GetAbsoluteFilePathString(),
            fileSystemProvider,
            environmentProvider,
            dispatcher);
    }
}