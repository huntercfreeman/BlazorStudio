using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.FileTemplates;

public class FileTemplateProvider : IFileTemplateProvider
{
    private List<IFileTemplate> _fileTemplates = new();

    public FileTemplateProvider()
    {
        _fileTemplates.Add(FileTemplateFacts.CSharpClass);
        _fileTemplates.Add(FileTemplateFacts.RazorMarkup);
        _fileTemplates.Add(FileTemplateFacts.RazorCodebehind);
    }

    public ImmutableArray<IFileTemplate> FileTemplates => _fileTemplates
        .ToImmutableArray();
}