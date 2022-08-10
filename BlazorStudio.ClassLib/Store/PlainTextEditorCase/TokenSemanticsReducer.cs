using Fluxor;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public class TokenSemanticsReducer
{
    [ReducerMethod]
    public static TokenSemanticsState ReduceUpdateTokenSemanticDescriptionAction(TokenSemanticsState previousTokenSemanticsState,
        UpdateTokenSemanticDescriptionsAction updateTokenSemanticDescriptionsAction)
    {
        return new(previousTokenSemanticsState.SemanticDescriptionsMap
            .SetItems(updateTokenSemanticDescriptionsAction.Tuple
                .Select(x => 
                    new KeyValuePair<TextTokenKey, SemanticDescription>(x.textTokenKey, x.semanticDescription))));
    }
}