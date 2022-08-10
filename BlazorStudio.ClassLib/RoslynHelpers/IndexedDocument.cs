using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorStudio.ClassLib.RoslynHelpers;

public class IndexedDocument
{
    public IndexedDocument(Document document, IAbsoluteFilePath absoluteFilePath)
    {
        Document = document;
        AbsoluteFilePath = absoluteFilePath;
    }
    
    public Document Document { get; set; }
    public IAbsoluteFilePath AbsoluteFilePath { get; set; }
    public List<PropertyDeclarationSyntax>? PropertyDeclarationSyntaxes { get; set; }
    public List<IdentifierNameSyntax>? IdentifierNameSyntaxes { get; set; }
}