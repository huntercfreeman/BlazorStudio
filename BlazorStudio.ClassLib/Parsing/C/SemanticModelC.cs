using BlazorTextEditor.RazorLib.Lexing;
using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Semantics;

namespace BlazorStudio.ClassLib.Parsing.C;

public class SemanticModelC : ISemanticModel
{
    private string? _text;
    private Lexer? _lexer;
    private Parser? _parser;
    private CompilationUnit? _compilationUnit;

    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        _text = model.GetAllText();

        var identifier = textSpan.GetText(_text);
        
        _lexer = new Lexer(_text);
        _lexer.Lex();

        _parser = new Parser(
            _lexer.SyntaxTokens,
            _text);
        
        _compilationUnit = _parser.Parse();

        if (_parser.VariableMap.TryGetValue(identifier, out var definition))
        {
            return new SymbolDefinition(
                model.ResourceUri,
                definition.IdentifierToken.BlazorStudioTextSpan.StartingIndexInclusive);
        }
        
        return null;
    }

    public void ManuallyRefreshSemanticModel(
        TextEditorModel model)
    {
        _text = model.GetAllText();

        _lexer = new Lexer(_text);
        _lexer.Lex();

        _parser = new Parser(
            _lexer.SyntaxTokens,
            _text);
        
        _compilationUnit = _parser.Parse();
    }
}