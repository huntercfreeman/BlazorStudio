using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.TreeViewImplementations;

namespace BlazorStudio.ClassLib.Menu;

public interface ICommonMenuOptionsFactory
{
    public MenuOptionRecord NewEmptyFile(
        IAbsoluteFilePath parentDirectory,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord NewTemplatedFile(
        NamespacePath parentDirectory,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord NewDirectory(
        IAbsoluteFilePath parentDirectory,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord DeleteFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord CopyFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord CutFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord RenameFile(
        IAbsoluteFilePath sourceAbsoluteFilePath,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord PasteClipboard(
        IAbsoluteFilePath directoryAbsoluteFilePath,
        Func<Task> onAfterCompletion);
    
    public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewNamespacePath? solutionNode,
        TreeViewNamespacePath projectNode,
        Func<Task> onAfterCompletion);
}