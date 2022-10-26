# BlazorStudio.ClassLib/Html

When the [TextTokenDisplay.razor](/BlazorStudio.RazorLib/PlainTextEditorCase/TextTokenDisplay.razor) component renders a
Whitespace token (
see: [PlainTextEditorStates.TextTokens.cs](/BlazorStudio.ClassLib/Store/PlainTextEditorCase/PlainTextEditorStates.TextTokens.cs))
that Whitespace token's plain text representation will not render correctly in HTML.

Therefore as an example, the space character gets converted "\&nbsp;" which is the HTML non breaking whitespace
character.