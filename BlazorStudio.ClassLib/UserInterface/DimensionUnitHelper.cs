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

            foreach (var widthDimensionUnit in dimensionUnitCalc)
            {
                builder.Append($"{widthDimensionUnit.Value}" +
                               $"{widthDimensionUnit.DimensionUnitKind.ToCssString()}" +
                               $"{Dimensions.STYLE_STRING_WHITESPACE}");
            }

            builder.Append($"){Dimensions.STYLE_STRING_DELIMITER}{Dimensions.STYLE_STRING_WHITESPACE}");
        }
    }
}