using System.Collections.Immutable;
using System.Text.Json;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.CSharpKeywords;

[FeatureState]
public record CSharpKeywords(ImmutableList<string> Keywords)
{
    public CSharpKeywords() : this(ImmutableList<string>.Empty)
    {
        // TODO: Do not hardcode the C# keyword paths
        var myUbuntuOsLocation = "/home/hunter/Repos/BlazorStudio/allCSharpKeywords.json";
        var myWindowsOsLocation = "C:\\Users\\hunte\\source\\BlazorStudio\\allCSharpKeywords.json";

        string jsonKeywords;

        if (File.Exists(myUbuntuOsLocation))
            jsonKeywords = File.ReadAllText(myUbuntuOsLocation);
        else
            jsonKeywords = File.ReadAllText(myWindowsOsLocation);

        var listOfKeywords = JsonSerializer.Deserialize<List<string>>(jsonKeywords);

        if (listOfKeywords is not null) Keywords = listOfKeywords.ToImmutableList();
    }
}