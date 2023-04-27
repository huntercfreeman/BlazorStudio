using BlazorStudio.ClassLib.Parsing.C.Symbols;

namespace BlazorStudio.ClassLib.Parsing.C;

public partial class CLanguageFacts
{
    public class Types
    {
        public static readonly Dictionary<string, ITypeSymbol> DefaultTypeMap = new()
        {
            {
                "int",
                new IntTypeSymbol()
            }
        };
    }
}