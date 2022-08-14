using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.Tests.RoslynTests;

public class LOAD_SOLUTION_TESTS : SOLUTION_STATE_TESTS
{
    // [Fact]
    // public void LOAD_SOLUTION_TEST()
    // {
    //     LoadSolution();
    //
    //     Assert.Equal(4, Solution.Projects.Count());
    // }
    //
    // [Fact]
    // public void GET_FILE_CONTENTS_TEST()
    // {
    //     LoadSolution();
    //
    //     var classLib = Solution.Projects
    //         .Single(p => p.Name == "ClassLib");
    //
    //     var class1 = classLib.Documents
    //         .Single(d => d.FilePath == "/home/hunter/RiderProjects/TestBlazorStudio/ClassLib/Class1.cs");
    //     
    //     var z = 2;
    // }
}