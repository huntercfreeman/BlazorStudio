namespace BlazorStudio.ClassLib.Parsing;

public record BlazorStudioTextSpan(
    int StartingIndexInclusive,
    int EndingIndexExclusive)
{
    public string GetText(string text)
    {
        return text.Substring(
            StartingIndexInclusive,
            EndingIndexExclusive - StartingIndexInclusive);
    }
}