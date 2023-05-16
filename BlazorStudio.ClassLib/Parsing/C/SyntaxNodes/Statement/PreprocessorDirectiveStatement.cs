using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;
using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Parsing.C.SyntaxNodes.Statement;

public class PreprocessorLibraryReferenceStatement : IStatementNode
{
    public PreprocessorLibraryReferenceStatement(
        ISyntaxToken includeDirectiveSyntaxToken,
        ISyntaxToken libraryReferenceSyntaxToken)
    {
        IncludeDirectiveSyntaxToken = includeDirectiveSyntaxToken;
        LibraryReferenceSyntaxToken = libraryReferenceSyntaxToken;

        Children = new ISyntax[]
        {
            IncludeDirectiveSyntaxToken,
            LibraryReferenceSyntaxToken,
        }.ToImmutableArray();
    }

    public ISyntaxToken IncludeDirectiveSyntaxToken { get; }
    public ISyntaxToken LibraryReferenceSyntaxToken { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorDirectiveStatement;
}