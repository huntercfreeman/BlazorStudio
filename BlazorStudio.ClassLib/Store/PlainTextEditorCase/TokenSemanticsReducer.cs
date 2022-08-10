using Fluxor;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public class TokenSemanticsReducer
{
    [ReducerMethod]
    public static TokenSemanticsState ReduceUpdateTokenSemanticDescriptionAction(TokenSemanticsState previousTokenSemanticsState,
        UpdateTokenSemanticDescriptionAction updateTokenSemanticDescriptionAction)
    {
        return new(previousTokenSemanticsState.SemanticDescriptionsMap.SetItem(updateTokenSemanticDescriptionAction.TextTokenKey,
            updateTokenSemanticDescriptionAction.SemanticDescription));
    }
}