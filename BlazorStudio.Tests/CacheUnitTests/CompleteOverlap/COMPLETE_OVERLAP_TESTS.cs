using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.Tests.CacheUnitTests.CompleteOverlap;

public class COMPLETE_OVERLAP_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public void CHUNK_ENCOMPASSES_REQUEST_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            0,
            30,
            0,
            50,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            10,
            10,
            10,
            20,
            CancellationToken.None);

        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\CompleteOverlap\\ChunkEncompassesRequest\\resultOne_CompleteOverlapChunkEncompassesRequest.txt");

        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\CompleteOverlap\\ChunkEncompassesRequest\\resultTwo_CompleteOverlapChunkEncompassesRequest.txt");

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
    public void REQUEST_ENCOMPASSES_CHUNK_TEST()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath(AbsoluteFilePathToThisCSharpProject +
                                                         "CacheTests\\THE TELL-TALE HEART.txt",
            false);

        var requestOne = new FileCoordinateGridRequest(
            10,
            10,
            10,
            20,
            CancellationToken.None);

        var requestTwo = new FileCoordinateGridRequest(
            0,
            30,
            0,
            50,
            CancellationToken.None);
        
        var expectedResultOne = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\CompleteOverlap\\RequestEncompassesChunk\\resultOne_CompleteOverlapChunkEncompassesRequest.txt");
        
        var expectedResultTwo = File.ReadAllText(AbsoluteFilePathToThisCSharpProject +
                                                 "CacheTestsData\\CompleteOverlap\\RequestEncompassesChunk\\resultTwo_CompleteOverlapChunkEncompassesRequest.txt");

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