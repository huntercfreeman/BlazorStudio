namespace BlazorStudio.ClassLib.Templates;

public static class DotNetTemplates
{
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
        return $@"<h1>{className.Replace(".razor", string.Empty)}</h1>;

@code{{
    
}}";
    }
}