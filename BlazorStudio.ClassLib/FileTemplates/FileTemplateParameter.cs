using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.FileTemplates;

public class FileTemplateParameter
{
    public FileTemplateParameter(
        string filename, 
        NamespacePath parentDirectory)
    {
        Filename = filename;
        ParentDirectory = parentDirectory;
    }

    public string Filename { get; }
    public NamespacePath ParentDirectory { get; }
}