using BlazorStudio.ClassLib.FileConstants;

namespace BlazorStudio.ClassLib.Templates;

public static class DotNetTemplates
{
    public static string GetTemplate(string extensionNoPeriod, string namespaceString, string className)
    {
        return extensionNoPeriod switch
        {
            ExtensionNoPeriodFacts.RAZOR_MARKUP => RazorMarkup(className),
            ExtensionNoPeriodFacts.RAZOR_CODEBEHIND => RazorCodebehind(namespaceString, className),
            ExtensionNoPeriodFacts.C_SHARP_CLASS => CSharpClass(namespaceString, className),
            _ => string.Empty
        };
    }
    
    public static string CSharpClass(string namespaceString, string className)
    {
        return $@"namespace {namespaceString};

public class {className}
{{
    
}}";
    }
    
    public static string RazorCodebehind(string namespaceString, string className)
    {
        return $@"namespace {namespaceString};

using Microsoft.AspNetCore.Components;

public partial class {className.Replace(".razor", string.Empty).Replace(".cs", string.Empty)} : ComponentBase
{{
    
}}";
    }
    
    public static string RazorMarkup(string className)
    {
        return $@"<h1>{className.Replace(".razor", string.Empty)}</h1>

@code{{
    
}}";
    }
}