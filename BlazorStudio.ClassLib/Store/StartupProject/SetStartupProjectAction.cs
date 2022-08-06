using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.StartupProject;

public record SetStartupProjectAction(IAbsoluteFilePath? ProjectAbsoluteFilePath);