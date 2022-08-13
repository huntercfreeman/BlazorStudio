namespace BlazorStudio.ClassLib.Templates;

public static class CSharpClassTemplate
{
    public static string WithInterpolation(string namespaceString, string className)
    {
        return $@"namespace {namespaceString};

public class {className}
{{
    
}}";
    }
        
}