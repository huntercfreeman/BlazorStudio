using BlazorStudio.ClassLib.DotNet.CSharp;

namespace BlazorStudio.ClassLib.ComponentRenderers.Types;

public interface ITreeViewCSharpProjectToProjectReferenceRendererType
{
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; }
}