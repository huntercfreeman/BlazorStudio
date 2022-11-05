using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.FileTemplates;

public interface IFileTemplateProvider
{
    public ImmutableArray<IFileTemplate> FileTemplates { get; }
}