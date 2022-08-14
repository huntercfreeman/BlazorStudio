using BlazorStudio.ClassLib;
using BlazorStudio.ClassLib.Clipboard;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.Tests.Clipboard;
using Fluxor;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorStudio.Tests.RoslynTests;

public class SOLUTION_STATE_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    protected VisualStudioInstance[] VisualStudioInstancesArray { get; private set; }    
    protected VisualStudioInstance VisualStudioInstance { get; private set; }    
    protected MSBuildWorkspace Workspace { get; private set; }    
    protected Solution Solution { get; private set; }    
    
    // protected void LoadSolution()
    // {
    //     VisualStudioInstancesArray = MSBuildLocator.QueryVisualStudioInstances().ToArray();
    //
    //     VisualStudioInstance = VisualStudioInstancesArray[0];
    //
    //     MSBuildLocator.RegisterInstance(VisualStudioInstance);
    //
    //     Workspace = MSBuildWorkspace.Create();
    //
    //     Dispatcher.Dispatch(new SetSolutionStateAction(Workspace, VisualStudioInstance));
    //
    //     Solution = Workspace
    //         .OpenSolutionAsync("/home/hunter/RiderProjects/TestBlazorStudio/TestBlazorStudio.sln")
    //         .Result;
    // }
}