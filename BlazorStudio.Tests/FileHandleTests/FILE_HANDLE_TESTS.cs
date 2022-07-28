using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;

namespace BlazorStudio.Tests.FileHandleTests;

public class FILE_HANDLE_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void READ_FILE_HANDLE_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(
            "C:\\Users\\hunte\\source\\repos\\dot-net-ide\\src\\extension.ts",
            false);

        var fileSystemProvider = new FileSystemProvider();

        var fileHandle = fileSystemProvider.Open(inputAbsoluteFilePath);

        var characterLengthOfLongestRow = (Int32) Math.Min(Int32.MaxValue, fileHandle.CharacterLengthOfLongestRow);
        
        var rowContent = fileHandle.Read(0, 0, 5, characterLengthOfLongestRow, 
            CancellationToken.None);

        var content = string.Join(string.Empty, rowContent);

        // TODO: Why does this Assertion not work (it fails and throws an exception)?
        Assert.Equal("import * as vscode from 'vscode';\r\nimport { ActiveDotNetSolutionFileContainer } from './ActiveDotNetSolutionFileContainer';\r\nimport { ConstantsFilePath } from './Constants/ConstantsFilePath';\r\nimport { FilePathStandardizer } from './FileSystem/FilePathStandardizer';\r\nimport { SolutionExplorerMessageTransporter } from './MessageHandlers/SolutionExplorer/SolutionExplorerMessageTransporter';",

content);
        
        fileHandle.Dispose();
    }
    
    [Fact]
    public void SINGLE_INSERT_FILE_HANDLE_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(
            "C:\\Users\\hunte\\source\\repos\\dot-net-ide\\src\\extension.ts",
            false);

        var fileSystemProvider = new FileSystemProvider();

        var fileHandle = fileSystemProvider.Open(inputAbsoluteFilePath);

        var characterLengthOfLongestRow = (Int32) Math.Min(Int32.MaxValue, fileHandle.CharacterLengthOfLongestRow);
        
        var firstRowContent = fileHandle.Read(0, 0, 5, characterLengthOfLongestRow, 
            CancellationToken.None);

        var firstContent = string.Join(string.Empty, firstRowContent);

        fileHandle.Edit.Insert(0, 0, "'Hello World!' - FileHandle");
        
        var secondRowContent = fileHandle.Read(0, 0, 5, characterLengthOfLongestRow, 
            CancellationToken.None);

        var secondContent = string.Join(string.Empty, secondRowContent);
        
        fileHandle.Dispose();
    }
    
    [Fact]
    public void SINGLE_REMOVE_FILE_HANDLE_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(
            "C:\\Users\\hunte\\source\\repos\\dot-net-ide\\src\\extension.ts",
            false);

        var fileSystemProvider = new FileSystemProvider();

        var fileHandle = fileSystemProvider.Open(inputAbsoluteFilePath);

        var characterLengthOfLongestRow = (Int32) Math.Min(Int32.MaxValue, fileHandle.CharacterLengthOfLongestRow);
        
        var firstRowContent = fileHandle.Read(0, 0, 5, characterLengthOfLongestRow, 
            CancellationToken.None);

        var firstContent = string.Join(string.Empty, firstRowContent);

        fileHandle.Edit.Remove(0, 0, 1, 0);
        
        var secondRowContent = fileHandle.Read(0, 0, 5, characterLengthOfLongestRow, 
            CancellationToken.None);

        var secondContent = string.Join(string.Empty, secondRowContent);
        
        fileHandle.Dispose();
    }
}