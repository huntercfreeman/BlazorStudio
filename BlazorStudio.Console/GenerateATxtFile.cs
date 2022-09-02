using System.Text;

namespace BlazorStudio.Console;

public static class GenerateATxtFile
{
    public static void Perform(string path)
    {
        var builder = new StringBuilder();

        var rows = 600;
        var columns = 600;

        var columnMarkerSpacing = 100;

        var spacer = ' ';

        for (int i = 0; i < rows; i++)
        {
            var rowMarker = $"// {i + 1}";

            builder.Append(rowMarker);

            for (int j = rowMarker.Length; j < columns; j++)
            {
                if (j % columnMarkerSpacing == 0)
                {
                    var columnMarker = $"col: {j + 1}";
                    builder.Append(columnMarker);

                    j += columnMarker.Length;
                }
                else
                {
                    builder.Append(spacer);
                }
            }

            builder.AppendLine();
        }

        File.WriteAllText(path, builder.ToString());
    }
}