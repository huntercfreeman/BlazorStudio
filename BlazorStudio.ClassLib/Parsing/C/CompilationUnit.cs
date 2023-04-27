using BlazorStudio.ClassLib.Parsing.C.SyntaxNodes;

namespace BlazorStudio.ClassLib.Parsing.C;

public class CompilationUnit
{
    public CompilationUnit(List<StatementNode> statementNodes)
    {
        StatementNodes = statementNodes;
    }

    public List<StatementNode> StatementNodes { get; }
}