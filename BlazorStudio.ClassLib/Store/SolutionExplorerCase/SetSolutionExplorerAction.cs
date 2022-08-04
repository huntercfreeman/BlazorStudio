using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.SolutionExplorerCase;

public record SetSolutionExplorerAction(IAbsoluteFilePath? SolutionAbsoluteFilePath);