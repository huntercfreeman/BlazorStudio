using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.Tests;

public class ROSLYN_TESTS
{
    [Fact]
    public async Task ANALYZE_SOLUTION()
    {
        const string targetPath = @"C:\Users\hunte\RiderProjects\ConsoleApp1\ConsoleApp1.sln";

        MSBuildLocator.RegisterDefaults();

        var workspace = MSBuildWorkspace.Create();

        var sln = await workspace.OpenSolutionAsync(targetPath);

        foreach (var project in sln.Projects)
        {
            Console.WriteLine(project.AssemblyName);
        }
    }
}