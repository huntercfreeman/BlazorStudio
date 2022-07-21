using System.Collections.Immutable;
using System.Text;

namespace BlazorStudio.ClassLib.UserInterface;

public static class DimensionUnitHelper
{
    public static void AppendToStyleString(StringBuilder builder, 
        IEnumerable<DimensionUnit> dimensionUnitCalc,
        string styleAttribute)
    {
        var immutableDimensionUnitCalc = dimensionUnitCalc.ToImmutableArray();

        if (immutableDimensionUnitCalc.Any())
        {
            builder.Append($"{styleAttribute}: calc(");

            for (var index = 0; index < immutableDimensionUnitCalc.Length; index++)
            {
                var dimensionUnit = immutableDimensionUnitCalc[index];

                if (dimensionUnit.IsDisabled)
                    continue;

                if (index != 0)
                {
                    builder.Append($"{Dimensions.STYLE_STRING_WHITESPACE}" +
                                   $"{dimensionUnit.DimensionUnitOperationKind.ToCssString()}" +
                                   $"{Dimensions.STYLE_STRING_WHITESPACE}");
                }

                builder.Append($"{Dimensions.STYLE_STRING_WHITESPACE}" +
                               $"{dimensionUnit.Value}" +
                               $"{dimensionUnit.DimensionUnitKind.ToCssString()}");

                if (index != immutableDimensionUnitCalc.Length - 1)
                {
                    builder.Append($"{Dimensions.STYLE_STRING_WHITESPACE}");
                }
            }

            builder.Append($"){Dimensions.STYLE_STRING_DELIMITER}{Dimensions.STYLE_STRING_WHITESPACE}");
        }
    }
}