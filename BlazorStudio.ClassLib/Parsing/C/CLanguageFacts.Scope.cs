using BlazorStudio.ClassLib.Parsing.C.Scope;

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
                },
                {
                    Types.Void.name,
                    Types.Void.type
                }
            };

            return new BoundScope(null, typeof(void), typeMap, new(), new());
        }
    }
}