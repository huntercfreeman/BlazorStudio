using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.RazorKeywords;

[FeatureState]
public record RazorKeywords(ImmutableList<Func<string, string>> KeywordFuncs)
{
    public RazorKeywords() : this(ImmutableList<Func<string, string>>.Empty)
    {
        KeywordFuncs = KeywordFuncs.Add(s => s.StartsWith("@") && !s.StartsWith("@@") 
            ? "pte_plain-text-editor-text-token-display-keyword"
            : string.Empty);
    }
}