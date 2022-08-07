// See https://aka.ms/new-console-template for more information




////////////////// PARSE C# KEYWORDS FROM https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/:


var content = File.ReadAllLines("C:\\BlazorStudioTestGround\\allCSharpKeywords.txt");

content = content
    .Select(x => x.Trim())
    .ToArray();

var json = System.Text.Json.JsonSerializer.Serialize(content);

File.WriteAllText("C:\\BlazorStudioTestGround\\allCSharpKeywords.json", json);

var z = 2;

////////////////// GENERATE A TXT FILE:

//using System.Text;

//var builder = new StringBuilder();

//var rows = 600;
//var columns = 600;

//var columnMarkerSpacing = 100;

//var spacer = ' ';

//for (int i = 0; i < rows; i++)
//{
//    var rowMarker = $"// {i + 1}";

//    builder.Append(rowMarker);

//    for (int j = rowMarker.Length; j < columns; j++)
//    {
//        if (j % columnMarkerSpacing == 0)
//        {
//            var columnMarker = $"col: {j + 1}";
//            builder.Append(columnMarker);

//            j += columnMarker.Length;
//        }
//        else
//        {
//            builder.Append(spacer);
//        }
//    }

//    builder.AppendLine();
//}

//File.WriteAllText("../testFile.txt", builder.ToString());

//Console.WriteLine("Done");

//await Task.Delay(1000);