using BlazorStudio.ClassLib.Parsing.C.BoundNodes.Statements;

namespace BlazorStudio.ClassLib.Parsing.C.Scope;

public class BoundScope
{
    public BoundScope(
        BoundScope? parent,
        Type? scopeReturnType,
        Dictionary<string, Type> typeMap,
        Dictionary<string, BoundFunctionDeclarationNode> functionDeclarationMap,
        Dictionary<string, BoundVariableDeclarationStatementNode> variableDeclarationMap)
    {
        Parent = parent;
        ScopeReturnType = scopeReturnType;
        TypeMap = typeMap;
        FunctionDeclarationMap = functionDeclarationMap;
        VariableDeclarationMap = variableDeclarationMap;
    }

    public BoundScope? Parent { get; }
    /// <summary>A <see cref="ScopeReturnType"/> with the value of "null" means refer to the <see cref="Parent"/> bound scope's <see cref="ScopeReturnType"/></summary>
    public Type? ScopeReturnType { get; }
    public Dictionary<string, Type> TypeMap { get; }
    public Dictionary<string, BoundFunctionDeclarationNode> FunctionDeclarationMap { get; }
    public Dictionary<string, BoundVariableDeclarationStatementNode> VariableDeclarationMap { get; }
}
