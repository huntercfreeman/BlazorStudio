using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.DotNet.CSharp;

public class CSharpProjectDependencies
{
    public CSharpProjectDependencies(NamespacePath cSharpProjectNamespacePath)
    {
        CSharpProjectNamespacePath = cSharpProjectNamespacePath;
    }
    
    public NamespacePath CSharpProjectNamespacePath { get; }
}