using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.DotNet.CSharp;

public class CSharpProjectNugetPackageReferences
{
    public CSharpProjectNugetPackageReferences(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }
    
    public NamespacePath CSharpProjectNamespacePath { get; }
}