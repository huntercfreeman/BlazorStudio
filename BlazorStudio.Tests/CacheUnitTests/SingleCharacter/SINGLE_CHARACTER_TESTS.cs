using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.Tests.CacheUnitTests.SingleCharacter;

public class SINGLE_CHARACTER_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void SINGLE_CHARACTER_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTestsData\\singleCharacter.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            0,
            30,
            0,
            17,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            5,
            5,
            10,
            20,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\SingleCharacter\\resultOne_SingleCharacter.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\SingleCharacter\\resultTwo_SingleCharacter.txt");

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            inputAbsoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        var actionOne = new MemoryMappedFileExactReadRequestAction(plainTextEditorKey,
            requestOne);

        Dispatcher.Dispatch(actionOne);

        var actualResultOne = plainTextEditor.GetPlainText();

        Assert.Equal(expectedResultOne, actualResultOne);

        var actionTwo = new MemoryMappedFileExactReadRequestAction(plainTextEditorKey,
            requestTwo);

        Dispatcher.Dispatch(actionTwo);

        var actualResultTwo = plainTextEditor.GetPlainText();

        Assert.Equal(expectedResultTwo, actualResultTwo);
    }
}