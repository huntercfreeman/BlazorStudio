using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.DotNet.CSharp;

public class CSharpProjectToProjectReferences
{
    public CSharpProjectToProjectReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }
    
    public NamespacePath CSharpProjectNamespacePath { get; }
}