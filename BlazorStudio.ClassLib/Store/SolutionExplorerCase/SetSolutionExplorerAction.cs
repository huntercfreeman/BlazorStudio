using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.SolutionExplorerCase;

public record SetSolutionExplorerAction(IAbsoluteFilePath? SolutionAbsoluteFilePath, SequenceKey SequenceKey);