using System.Collections.Immutable;
using Fluxor;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

[FeatureState]
public record TokenSemanticsState(ImmutableDictionary<TextTokenKey, SemanticDescription> SemanticDescriptionsMap)
{
    public TokenSemanticsState() 
        : this(ImmutableDictionary<TextTokenKey, SemanticDescription>.Empty)
    {
        
    }
}