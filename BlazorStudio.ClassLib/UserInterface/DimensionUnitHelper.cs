using System.Text;

namespace BlazorStudio.ClassLib.UserInterface;

public static class DimensionUnitHelper
{
    public static void AppendToStyleString(StringBuilder builder, 
        List<DimensionUnit> dimensionUnitCalc,
        string styleAttribute)
    {
        if (dimensionUnitCalc.Any())
        {
            builder.Append($"{styleAttribute}: calc(");

            for (var index = 0; index < dimensionUnitCalc.Count; index++)
            {
                var dimensionUnit = dimensionUnitCalc[index];

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

                if (index != dimensionUnitCalc.Count - 1)
                {
                    builder.Append($"{Dimensions.STYLE_STRING_WHITESPACE}");
                }
            }

            builder.Append($"){Dimensions.STYLE_STRING_DELIMITER}{Dimensions.STYLE_STRING_WHITESPACE}");
        }
    }
}