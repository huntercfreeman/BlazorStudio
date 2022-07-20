using System.Text;
using BlazorStudio.Shared.FileSystem.Classes;
using BlazorStudio.Shared.FileSystem.Files.DotNet.CSharp;
using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet;
using BlazorStudio.Shared.FileSystem.Files.Interfaces.Files.DotNet.CSharp;
using BlazorStudio.Shared.FileSystem.Interfaces;

namespace BlazorStudio.Shared.FileSystem.Files.DotNet;

public class DotNetSolutionFileModel : FileModelBase, IDotNetSolutionFileModel
{
    private const string PROJECT_DEFINITION_IDENTIFIER = "Project(";
    private const string PROJECT_DEFINITION_START_OF_GUID_ONE_IDENTIFIER = "\"{";

    private readonly StringBuilder _tokenBuilder = new();

    public DotNetSolutionFileModel(IFileDescriptor? fileDescriptor, IAbsoluteFilePath absoluteFilePath)
        : base(fileDescriptor, absoluteFilePath)
    {
    }

    public List<ICSharpProjectFileModel> CSharpProjects { get; set; } = new();
    public bool CSharpProjectsInitiallyLoaded { get; private set; }

    public void LoadCSharpProjects()
    {
        using FileContentStreamer fileContentStreamer = new FileContentStreamer(AbsoluteFilePath!, "Project(".Length);

        char currentCharacter;

        while ((currentCharacter = fileContentStreamer.ConsumeCharacter()) != '\0')
        {
            switch (currentCharacter)
            {
                case 'P':
                    _tokenBuilder.Append(currentCharacter);

                    if (fileContentStreamer.MatchSubstring(new string(PROJECT_DEFINITION_IDENTIFIER.Skip(1).ToArray())))
                    {
                        HandleProjectDefinition(fileContentStreamer);
                    }
                    break;
                default:
                    break;
            }
        }

        CSharpProjectsInitiallyLoaded = true;
    }

    public async Task LoadCSharpProjectsAsync()
    {
        CSharpProjectsInitiallyLoaded = true;

        throw new NotImplementedException();
    }

    private void HandleProjectDefinition(FileContentStreamer fileContentStreamer)
    {
        var guidOne = HandleGuidOne(fileContentStreamer);

        // ...-BF4B-00C04F79EFBC}") = "BlazorApp...
        //                      ^
        fileContentStreamer.ConsumeUntilPeekCharacter('"');
        fileContentStreamer.ConsumeCharacter();
        fileContentStreamer.ConsumeUntilPeekCharacter('"');
        fileContentStreamer.ConsumeCharacter();

        var displayName = HandleProjectDisplayName(fileContentStreamer);

        // ...zorApp1", "BlazorApp1\BlazorApp1.csproj...
        //           ^
        fileContentStreamer.ConsumeCharacter();
        fileContentStreamer.ConsumeUntilPeekCharacter('"');
        fileContentStreamer.ConsumeCharacter();

        var projectAbsoluteFilePath = HandleProjectRelativePathFromSolution(fileContentStreamer);

        // ...App1.csproj", "{20008755-14...
        //               ^
        fileContentStreamer.ConsumeCharacter();
        fileContentStreamer.ConsumeUntilPeekCharacter('{');
        fileContentStreamer.ConsumeCharacter();

        var guidTwo = HandleGuidTwo(fileContentStreamer);

        CSharpProjects.Add(
            new CSharpProjectFileModel(null, projectAbsoluteFilePath, guidOne, guidTwo, displayName));
    }

    private Guid HandleGuidOne(FileContentStreamer fileContentStreamer)
    {
        if (fileContentStreamer.MatchSubstring(PROJECT_DEFINITION_START_OF_GUID_ONE_IDENTIFIER))
        {
            string guidOneAsString = fileContentStreamer.ConsumeUntilPeekCharacter('}');

            if (!Guid.TryParse(guidOneAsString, out var guidOneAsGuid))
            {
                throw new ApplicationException($"The dotnet solution file named: " +
                    $"{AbsoluteFilePath.FilenameWithExtension} " +
                    $"was malformed at position: {fileContentStreamer.GetStreamPositionIndex}.");
            }
            else
            {
                return guidOneAsGuid;
            }
        }
        else
        {
            throw new ApplicationException($"The dotnet solution file named: " +
                $"{AbsoluteFilePath.FilenameWithExtension} " +
                $"was malformed at position: {fileContentStreamer.GetStreamPositionIndex}.");
        }
    }

    private string HandleProjectDisplayName(FileContentStreamer fileContentStreamer)
    {
        string displayName = fileContentStreamer.ConsumeUntilPeekCharacter('"');

        return displayName;
    }

    private AbsoluteFilePath HandleProjectRelativePathFromSolution(FileContentStreamer fileContentStreamer)
    {
        string relativePathFromSolution = fileContentStreamer.ConsumeUntilPeekCharacter('"');

        IRelativeFilePath relativeFilePath = new RelativeFilePath(relativePathFromSolution,
            false);

        return new AbsoluteFilePath(AbsoluteFilePath, relativeFilePath);
    }

    private Guid HandleGuidTwo(FileContentStreamer fileContentStreamer)
    {
        string guidTwoAsString = fileContentStreamer.ConsumeUntilPeekCharacter('}');

        if (!Guid.TryParse(guidTwoAsString, out var guidTwoAsGuid))
        {
            throw new ApplicationException($"The dotnet solution file named: " +
                $"{AbsoluteFilePath.FilenameWithExtension} " +
                $"was malformed at position: {fileContentStreamer.GetStreamPositionIndex}.");
        }
        else
        {
            return guidTwoAsGuid;
        }
    }
}
