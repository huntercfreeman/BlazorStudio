using Fluxor;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.RoslynWorkspaceState;

/// <summary>
/// Thank you to "https://www.reddit.com/user/LloydAtkinson/"
/// https://www.reddit.com/r/dotnet/comments/vekc4g/a_conversation_about_parsing_solution_files_sln/
/// I was making a Visual Studio Code extension and he
/// left a very interesting idea that I used for BlazorStudio.
/// </summary>
[FeatureState]
public record RoslynWorkspaceState(MSBuildWorkspace? MSBuildWorkspace,
    VisualStudioInstance? VisualStudioInstance,
    IAbsoluteFilePath? MsBuildAbsoluteFilePath)
{
    public RoslynWorkspaceState() : this(default,
        default,
        default)
    {
    }
}