using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.Tests.CacheUnitTests.PartialOverlap;

public class PARTIAL_OVERLAP_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void EAST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
            "TestData\\CacheTestsData\\THE TELL-TALE HEART.txt",
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
                                                 "TestData\\CacheTestsData\\PartialOverlap\\East\\resultOne_PartialOverlap_East.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "TestData\\CacheTestsData\\PartialOverlap\\East\\resultTwo_PartialOverlap_East.txt");

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

    [Fact]
    public void EAST_WEST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
            "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            0,
            30,
            15,
            5,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            5,
            5,
            0,
            40,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\EastWest\\resultOne_PartialOverlap_EastWest.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\EastWest\\resultTwo_PartialOverlap_EastWest.txt");

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

    [Fact]
    public void NORTH_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            0,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            0,
            30,
            5,
            15,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\North\\resultOne_PartialOverlap_North.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\North\\resultTwo_PartialOverlap_North.txt");

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

    [Fact]
    public void NORTH_EAST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            0,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            0,
            30,
            5,
            30,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\NorthEast\\resultOne_PartialOverlap_NorthEast.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\NorthEast\\resultTwo_PartialOverlap_NorthEast.txt");

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

    [Fact]
    public void NORTH_SOUTH_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            0,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            0,
            60,
            5,
            15,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\NorthSouth\\resultOne_PartialOverlap_NorthSouth.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\NorthSouth\\resultTwo_PartialOverlap_NorthSouth.txt");

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

    [Fact]
    public void NORTH_WEST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            20,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            5,
            15,
            9,
            15,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\NorthWest\\resultOne_PartialOverlap_NorthWest.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\NorthWest\\resultTwo_PartialOverlap_NorthWest.txt");

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

    [Fact]
    public void SOUTH_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            20,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            20,
            60,
            25,
            10,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\South\\resultOne_PartialOverlap_South.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\South\\resultTwo_PartialOverlap_South.txt");

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

    [Fact]
    public void SOUTH_EAST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            20,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            20,
            60,
            25,
            30,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\SouthEast\\resultOne_PartialOverlap_SouthEast.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\SouthEast\\resultTwo_PartialOverlap_SouthEast.txt");

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

    [Fact]
    public void SOUTH_WEST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            20,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            20,
            60,
            0,
            30,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\SouthWest\\resultOne_PartialOverlap_SouthWest.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\SouthWest\\resultTwo_PartialOverlap_SouthWest.txt");

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

    [Fact]
    public void WEST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            15,
            30,
            20,
            25,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            20,
            1,
            0,
            30,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\West\\resultOne_PartialOverlap_West.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\PartialOverlap\\West\\resultTwo_PartialOverlap_West.txt");

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