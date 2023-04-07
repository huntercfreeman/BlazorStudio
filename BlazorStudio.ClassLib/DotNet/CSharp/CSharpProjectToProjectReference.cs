using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.DotNet.CSharp;

public class CSharpProjectToProjectReference
{
    public CSharpProjectToProjectReference(IAbsoluteFilePath absoluteFilePath)
    {
        AbsoluteFilePath = absoluteFilePath;
    }
    
    public IAbsoluteFilePath AbsoluteFilePath { get; }
}