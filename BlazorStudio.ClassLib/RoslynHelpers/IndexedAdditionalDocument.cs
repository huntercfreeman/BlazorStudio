using BlazorStudio.ClassLib.FileSystem.Classes;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class IndexedAdditionalDocument
{
    public IndexedAdditionalDocument(TextDocument textDocument, AbsoluteFilePathDotNet absoluteFilePathDotNet)
    {
        TextDocument = textDocument;
        AbsoluteFilePathDotNet = absoluteFilePathDotNet;
    }
    
    public TextDocument TextDocument { get; set; }
    public AbsoluteFilePathDotNet AbsoluteFilePathDotNet { get; set; }
    public GeneralSyntaxCollector GeneralSyntaxCollector { get; set; }
}