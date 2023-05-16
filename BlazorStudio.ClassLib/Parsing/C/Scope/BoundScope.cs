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
        Dictionary<string, Type> typeMap)
    {
        Parent = parent;
        TypeMap = typeMap;
    }

    public Dictionary<string, Type> TypeMap { get; }
    public BoundScope? Parent { get; }
}
