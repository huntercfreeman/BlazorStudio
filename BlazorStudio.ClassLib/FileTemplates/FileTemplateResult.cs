using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.FileTemplates;

public class FileTemplateResult
{
    public FileTemplateResult(
        NamespacePath fileNamespacePath, 
        string contents)
    {
        FileNamespacePath = fileNamespacePath;
        Contents = contents;
    }

    public NamespacePath FileNamespacePath { get; }
    public string Contents { get; }
}