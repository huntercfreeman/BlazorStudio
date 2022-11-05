using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.FileTemplates;

public interface IFileTemplate
{
    /// <summary>
    /// Name displayed to a user
    /// </summary>
    public string DisplayName { get; }
    /// <summary>
    /// Name to disambiguate internally
    /// </summary>
    public string CodeName { get; }
    public FileTemplateKind FileTemplateKind { get; }
    /// <summary>
    /// Func&lt;string filename, bool isApplicable&gt;
    /// </summary>
    public Func<string, bool> IsApplicable { get; }
    /// <summary>
    /// Allows a user to type "MenuComponent.razor"
    /// and be prompted with a checkbox if they would like
    /// to add a codebehind named "MenuComponent.razor.cs"
    /// </summary>
    public Func<
            string, 
            ImmutableArray<(IFileTemplate fileTemplate, bool initialCheckedState)>> 
        RelatedFileTemplates { get; }
    /// <summary>
    /// Func&lt;string filename, NamespacePath parentDirectory, Task writeOutFileTask&gt;
    /// </summary>
    public Func<FileTemplateParameter, FileTemplateResult> ConstructFileContents { get; }
}