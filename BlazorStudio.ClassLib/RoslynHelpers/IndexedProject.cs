using BlazorStudio.ClassLib.FileSystem.Classes;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class IndexedProject
{
    public IndexedProject(Project project, AbsoluteFilePathDotNet absoluteFilePathDotNet)
    {
        Project = project;
        AbsoluteFilePathDotNet = absoluteFilePathDotNet;
    }

    public Project Project { get; set; }
    public AbsoluteFilePathDotNet AbsoluteFilePathDotNet { get; set; }
}