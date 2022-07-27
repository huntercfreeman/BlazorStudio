using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.Tests.CacheUnitTests.EmptyFile;

public class EMPTY_FILE_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void EMPTY_FILE_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(
            "C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            inputAbsoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        var requestOne = new FileCoordinateGridRequest(
            10,
            10,
            10,
            20,
            CancellationToken.None);


        var actionOne = new MemoryMappedFileExactReadRequestAction(plainTextEditorKey,
            requestOne);

        Dispatcher.Dispatch(actionOne);

        var actualResultOne = plainTextEditor.GetPlainText();

        var expectedResultOne = File.ReadAllText("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultOne_CACHE_NORTH_OVERLAP_TXT.txt");

        Assert.Equal(expectedResultOne, actualResultOne);

        var requestTwo = new FileCoordinateGridRequest(
            0,
            30,
            0,
            50,
            CancellationToken.None);

        var actionTwo = new MemoryMappedFileExactReadRequestAction(plainTextEditorKey,
            requestTwo);

        Dispatcher.Dispatch(actionTwo);

        var actualResultTwo = plainTextEditor.GetPlainText();

        var expectedResultTwo = File.ReadAllText("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultTwo_CACHE_NORTH_OVERLAP_TXT.txt");

        Assert.Equal(expectedResultTwo, actualResultTwo);
    }
}