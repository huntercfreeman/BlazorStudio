using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.GitCase;

public partial  record GitState
{
    public record SetGitFolderAbsoluteFilePathAction(IAbsoluteFilePath? GitFolderAbsoluteFilePath);
}