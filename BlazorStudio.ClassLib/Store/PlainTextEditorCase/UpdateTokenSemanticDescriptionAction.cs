namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public record UpdateTokenSemanticDescriptionsAction(List<(TextTokenKey textTokenKey, SemanticDescription semanticDescription)> Tuple);
