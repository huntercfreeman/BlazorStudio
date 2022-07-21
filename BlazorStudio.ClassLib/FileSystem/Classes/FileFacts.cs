namespace BlazorStudio.ClassLib.FileSystem.Classes;

public static class FileFacts
{
    public static class FileExtensions
    {
        public const string CSharpFile = "cs";
        public const string CSharpProject = "csproj";
        public const string Json = "json";
        public const string DotnetSolution = "sln";
        public const string Text = "txt";
        public const string Unrecognized = "";

        public static readonly List<string> Listing = new()
        {
            CSharpFile,
            CSharpProject,
            Json, 
            DotnetSolution,
            Text,
            Unrecognized 
        };
    }

    public static class ReservedFileNames
    {
        public const string CSHARP_BIN_FOLDER = "bin";
        public const string CSHARP_OBJ_FOLDER = "obj";

        public static readonly List<string> HiddenCSharpFileNames = new()
        {
            CSHARP_BIN_FOLDER,
            CSHARP_OBJ_FOLDER
        };
    }
}