namespace BlazorStudio.ClassLib.CommandLine;

public static class CommandLineHelper
{
    public static string QuoteValue(string parameter)
    {
        return $"\"{parameter}\"";
    }
}