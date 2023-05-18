using BlazorStudio.ClassLib.Parsing.C.BoundNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStudio.ClassLib.Parsing.C.Scope;

public class BoundScope
{
    public BoundScope(
        BoundScope? parent,
        Dictionary<string, Type> typeMap,
        Dictionary<string, BoundFunctionDeclarationNode> functionDeclarationMap,
        Dictionary<string, BoundVariableDeclarationStatementNode> variableDeclarationMap)
    {
        Parent = parent;
        TypeMap = typeMap;
        FunctionDeclarationMap = functionDeclarationMap;
        VariableDeclarationMap = variableDeclarationMap;
    }

    public Dictionary<string, Type> TypeMap { get; }
    public Dictionary<string, BoundFunctionDeclarationNode> FunctionDeclarationMap { get; }
    public Dictionary<string, BoundVariableDeclarationStatementNode> VariableDeclarationMap { get; }
    public BoundScope? Parent { get; }
}
