using BlazorStudio.ClassLib.Parsing.C.BoundNodes;
using BlazorStudio.ClassLib.Parsing.C.Scope;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.ClassLib.Parsing.C;

public partial class CLanguageFacts
{
    public class Scope
    {
        public static BoundScope GetInitialGlobalScope()
        {
            var typeMap = new Dictionary<string, Type>
            {
                {
                    Types.Int.name,
                    Types.Int.type
                },
                {
                    Types.String.name,
                    Types.String.type
                }
            };

            return new BoundScope(null, typeMap);
        }
    }
}