using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.FileTemplates;

public static class FileTemplateFacts
{
    public static readonly IFileTemplate CSharpClass = new FileTemplate(
        "C# Class",
        "bstudio-c-sharp-class",
        FileTemplateKind.CSharp,
        filename => filename.EndsWith(".cs"),
        filename => ImmutableArray<(IFileTemplate fileTemplate, bool initialCheckedState)>.Empty, 
        CSharpClassCreateFileFunc);
    
    public static readonly IFileTemplate RazorMarkup = new FileTemplate(
        "Razor markup",
        "bstudio-razor-markup-class",
        FileTemplateKind.Razor,
        filename => filename.EndsWith(".razor"),
        filename => new[] { (RazorCodebehind, true) }.ToImmutableArray(),
        RazorMarkupCreateFileFunc);

    public static readonly IFileTemplate RazorCodebehind = new FileTemplate(
        "Razor codebehind",
        "bstudio-razor-codebehind-class",
        FileTemplateKind.Razor,
        filename => filename.EndsWith(".razor.cs"),
        filename => ImmutableArray<(IFileTemplate fileTemplate, bool initialCheckedState)>.Empty,
        RazorCodebehindCreateFileFunc);
    
    /// <summary>
    /// namespace BlazorStudio.ClassLib.FileTemplates;
    ///
    /// public class Asdf
    /// {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult CSharpClassCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"namespace {namespaceString};{Environment.NewLine}");
            
            templateBuilder.Append(
                Environment.NewLine);
            
            templateBuilder.Append(
                $"public class {fileNameNoExtension}{Environment.NewLine}");
            
            templateBuilder.Append(
                $"{{{Environment.NewLine}");
            
            templateBuilder.Append(
                $"\t{Environment.NewLine}");
            
            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }
        
        var emptyFileAbsoluteFilePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsoluteFilePath
                                                  .GetAbsoluteFilePathString() +
                                              fileTemplateParameter.Filename;

        // Create AbsoluteFilePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
            emptyFileAbsoluteFilePathString, 
            false);

        var templatedFileContent = GetContent(
            emptyFileAbsoluteFilePath.FileNameNoExtension,
            fileTemplateParameter.ParentDirectory.Namespace);
        
        var templatedFileFileAbsoluteFilePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsoluteFilePath
                                                     .GetAbsoluteFilePathString() +
                                                 emptyFileAbsoluteFilePath.FileNameNoExtension;
        
        var templatedFileAbsoluteFilePath = new AbsoluteFilePath(
            templatedFileFileAbsoluteFilePathString, 
            false);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsoluteFilePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }
    
    /// <summary>
    /// <h3>Asdf</h3>
    /// 
    /// @code {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult RazorMarkupCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"<h3>{fileNameNoExtension}</h3>{Environment.NewLine}");
            
            templateBuilder.Append(
                Environment.NewLine);
            
            templateBuilder.Append(
                $"@code {{{Environment.NewLine}");
            
            templateBuilder.Append(
                $"\t{Environment.NewLine}");
            
            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }
        
        var emptyFileAbsoluteFilePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsoluteFilePath
                                                  .GetAbsoluteFilePathString() +
                                              fileTemplateParameter.Filename;

        // Create AbsoluteFilePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
            emptyFileAbsoluteFilePathString, 
            false);

        var templatedFileContent = GetContent(
            emptyFileAbsoluteFilePath.FileNameNoExtension,
            fileTemplateParameter.ParentDirectory.Namespace);
        
        var templatedFileFileAbsoluteFilePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsoluteFilePath
                                                     .GetAbsoluteFilePathString() +
                                                 emptyFileAbsoluteFilePath.FileNameNoExtension;
        
        var templatedFileAbsoluteFilePath = new AbsoluteFilePath(
            templatedFileFileAbsoluteFilePathString, 
            false);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsoluteFilePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }
    
    /// <summary>
    /// using Microsoft.AspNetCore.Components;
    /// 
    /// namespace BlazorStudio.RazorLib.Menu;
    /// 
    /// public partial class Asdf : ComponentBase
    /// {
    ///     
    /// }
    /// </summary>
    private static FileTemplateResult RazorCodebehindCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"using Microsoft.AspNetCore.Components;{Environment.NewLine}");
            
            templateBuilder.Append(
                Environment.NewLine);
            
            templateBuilder.Append(
                $"namespace {namespaceString};{Environment.NewLine}");
            
            templateBuilder.Append(
                Environment.NewLine);
            
            templateBuilder.Append(
                $"public partial class {fileNameNoExtension} : ComponentBase{Environment.NewLine}");
            
            templateBuilder.Append(
                $"{{{Environment.NewLine}");
            
            templateBuilder.Append(
                $"\t{Environment.NewLine}");
            
            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }
        
        var emptyFileAbsoluteFilePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsoluteFilePath
                                                  .GetAbsoluteFilePathString() +
                                              fileTemplateParameter.Filename;

        // Create AbsoluteFilePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
            emptyFileAbsoluteFilePathString, 
            false);

        var templatedFileContent = GetContent(
            emptyFileAbsoluteFilePath.FileNameNoExtension,
            fileTemplateParameter.ParentDirectory.Namespace);
        
        var templatedFileFileAbsoluteFilePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsoluteFilePath
                                                     .GetAbsoluteFilePathString() +
                                                 emptyFileAbsoluteFilePath.FileNameNoExtension;
        
        var templatedFileAbsoluteFilePath = new AbsoluteFilePath(
            templatedFileFileAbsoluteFilePathString, 
            false);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsoluteFilePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }
}