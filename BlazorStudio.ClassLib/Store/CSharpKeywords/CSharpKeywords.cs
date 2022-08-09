using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.CSharpKeywords;

[FeatureState]
public record CSharpKeywords(ImmutableList<string> Keywords)
{
    public CSharpKeywords() : this(ImmutableList<string>.Empty)
    {
        var jsonKeywords = File.ReadAllText("/home/hunter/Repos/BlazorStudio/allCSharpKeywords.json");
        
        var listOfKeywords = System.Text.Json.JsonSerializer.Deserialize<List<string>>(jsonKeywords);
        
        if (listOfKeywords is not null)
        {
            Keywords = listOfKeywords.ToImmutableList();
        }
    }
}