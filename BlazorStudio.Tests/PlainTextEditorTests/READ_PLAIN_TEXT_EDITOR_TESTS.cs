using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.Tests.PlainTextEditorTests;


/// <summary>
/// In this file I use absolute file paths "string absoluteFilePathString" as parameters
/// for tests. This is not ideal as the tests will fail
/// if someone other than I runs the tests. But I do not want to take time
/// writing logic for relative paths at this moment in time.
/// </summary>
public class READ_PLAIN_TEXT_EDITOR_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public async Task READ_EMPTY_FILE()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        var absoluteFilePath = 
            new AbsoluteFilePath("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/EmptyFile.txt",
                false);

        var fileSystemProvider = new FileSystemProvider();
        
        await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey, 
                absoluteFilePath, 
                fileSystemProvider, 
                CancellationToken.None),
            PlainTextEditorStateWrap);
        
        Assert.Single(PlainTextEditorStateWrap.Value.Map);
        Assert.Single(PlainTextEditorStateWrap.Value.Array);

        var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
        
        Assert.Equal(string.Empty, editor.GetPlainText());

        bool expectedUseCarriageReturnNewLine = false;
        Assert.Equal(expectedUseCarriageReturnNewLine, editor.UseCarriageReturnNewLine);
    }
    
    [Theory]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/NewLine.txt", "\n", false)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/CarriageReturnNewLine.txt", "\r\n", true)]
    public async Task READ_LINE_ENDING(string absoluteFilePathString, string lineEnding, bool expectedUseCarriageReturnNewLine)
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        var absoluteFilePath = new AbsoluteFilePath(absoluteFilePathString, false);

        var fileSystemProvider = new FileSystemProvider();
        
        await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey, 
                absoluteFilePath, 
                fileSystemProvider, 
                CancellationToken.None),
            PlainTextEditorStateWrap);
        
        Assert.Single(PlainTextEditorStateWrap.Value.Map);
        Assert.Single(PlainTextEditorStateWrap.Value.Array);

        var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
        
        Assert.Equal(lineEnding, editor.GetPlainText());
        Assert.Equal(expectedUseCarriageReturnNewLine, editor.UseCarriageReturnNewLine);
    }

    // /// <summary>
    // /// This took 5 minutes and 35 seconds to finish (needs optimization)
    // /// </summary>
    // [Fact]
    // public async Task READ_LARGE_FILE_AS_TOKENIZED()
    // {
    //     var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
    //
    //     var absoluteFilePath =
    //         new AbsoluteFilePath("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/Hamlet_ Entire Play.html",
    //             false);
    //
    //     var fileSystemProvider = new FileSystemProvider();
    //
    //     await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
    //             absoluteFilePath,
    //             fileSystemProvider,
    //             CancellationToken.None),
    //         PlainTextEditorStateWrap);
    //
    //     Assert.Single(PlainTextEditorStateWrap.Value.Map);
    //     Assert.Single(PlainTextEditorStateWrap.Value.Array);
    //
    //     var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
    // }
}