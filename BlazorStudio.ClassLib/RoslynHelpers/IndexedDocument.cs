using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class IndexedDocument
{
    public IndexedDocument(Document document, AbsoluteFilePathDotNet absoluteFilePathDotNet)
    {
        Document = document;
        AbsoluteFilePathDotNet = absoluteFilePathDotNet;
    }
    
    public Document Document { get; set; }
    public AbsoluteFilePathDotNet AbsoluteFilePathDotNet { get; set; }
    public GeneralSyntaxCollector GeneralSyntaxCollector { get; set; }
}