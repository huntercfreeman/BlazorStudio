using BlazorStudio.ClassLib.Parsing.C.Symbols;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C;

public class Variable : ISyntax
{
    public Variable(
        ITypeSymbol typeSymbol,
        IdentifierToken identifierToken)
    {
        TypeSymbol = typeSymbol;
        IdentifierToken = identifierToken;
    }

    public IdentifierToken IdentifierToken { get; }
    public ITypeSymbol TypeSymbol { get; }
    public object? Value { get; private set; }
    public bool IsInitialized { get; private set; }

    public void SetValue(object? value)
    {
        Value = value;
        IsInitialized = true;
    }

    public SyntaxKind SyntaxKind => SyntaxKind.Variable;
}