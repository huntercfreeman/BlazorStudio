using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Semantics;

namespace BlazorStudio.ClassLib.Parsing.C;

public class SemanticModelC : ISemanticModel
{
    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        string text = model.GetAllText();

        var identifier = textSpan.GetText(text);
        
        var lexer = new Lexer(text);
        lexer.Lex();

        var parser = new Parser(lexer.SyntaxTokens);
        
        var root = parser.Parse();

        return null;
    }
}