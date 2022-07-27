using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.Tests.CacheUnitTests.NoOverlap;

/// <summary>
/// For these tests 'EAST_TEST' describes the position of the request relative to the existing chunk.
/// </summary>
public class NO_OVERLAP_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void EAST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
            "CacheTests\\THE TELL-TALE HEART.txt",
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
            25,
            20,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultOne_CACHE_NORTH_OVERLAP_TXT.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultTwo_CACHE_NORTH_OVERLAP_TXT.txt");

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
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultOne_CACHE_NORTH_OVERLAP_TXT.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultTwo_CACHE_NORTH_OVERLAP_TXT.txt");

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
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultOne_CACHE_NORTH_OVERLAP_TXT.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultTwo_CACHE_NORTH_OVERLAP_TXT.txt");

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
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultOne_CACHE_NORTH_OVERLAP_TXT.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultTwo_CACHE_NORTH_OVERLAP_TXT.txt");

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
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultOne_CACHE_NORTH_OVERLAP_TXT.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultTwo_CACHE_NORTH_OVERLAP_TXT.txt");

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
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultOne_CACHE_NORTH_OVERLAP_TXT.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTests\\CACHE_NORTH_OVERLAP_TXT\\resultTwo_CACHE_NORTH_OVERLAP_TXT.txt");

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
        throw new NotImplementedException();
    }
    
    [Fact]
    public void SOUTH_EAST_TEST()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void SOUTH_WEST_TEST()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void WEST_TEST()
    {
        throw new NotImplementedException();
    }
}