using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

public class InputFilePattern
{
    public InputFilePattern(
        string patternName,
        Func<IAbsoluteFilePath, bool> matchesPattern)
    {
        PatternName = patternName;
        MatchesPattern = matchesPattern;
    }

    public string PatternName { get; }
    public Func<IAbsoluteFilePath, bool> MatchesPattern { get; }
}