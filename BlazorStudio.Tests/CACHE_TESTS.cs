using System.IO.MemoryMappedFiles;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.Tests;

public class CACHE_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private double _paddingInPixels = 250;

    [Fact]
    public void CACHE_SINGLE_CHARACTER_TXT()
    {
        var inputAbsoluteFilePath = new AbsoluteFilePath("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\singleCharacter.txt",
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            inputAbsoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            0,
            0,
            0,
            500,
            500);

        var actualResultOne = plainTextEditor.GetPlainText();

        var expectedResultOne = File.ReadAllText("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\CACHE_SINGLE_CHARACTER_TXT\\resultOne_CACHE_SINGLE_CHARACTER_TXT.txt");

        Assert.Equal(expectedResultOne, actualResultOne);

        // Ensure the cache is reused
        for (int i = 0; i < 10; i++)
        {
            DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
                0,
                0,
                0,
                0,
                Math.Ceiling(WidthOfEachCharacterInPixels) * (i),
                Math.Ceiling(HeightOfEachRowInPixels) * (i));
        }

        var actualResultTwo = plainTextEditor.GetPlainText();

        var expectedResultTwo = File.ReadAllText("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\CACHE_SINGLE_CHARACTER_TXT\\resultTwo_CACHE_SINGLE_CHARACTER_TXT.txt");

        Assert.Equal(1, plainTextEditor.CacheCount);

        Assert.Equal(expectedResultTwo, actualResultTwo);
    }
    
    [Fact]
    public void CACHE_NORTH_OVERLAP_TXT()
    {
        var absoluteFilePath = new AbsoluteFilePath("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\PartiallyOverlappedChunks\\THE TELL-TALE HEART.txt",
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            absoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            Math.Ceiling(HeightOfEachRowInPixels) * (13),
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (25),
            Math.Ceiling(HeightOfEachRowInPixels) * (25));

        var resultOne = plainTextEditor.GetPlainText();

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            0,
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (25),
            Math.Ceiling(HeightOfEachRowInPixels) * (17));

        var resultTwo = plainTextEditor.GetPlainText();

        Assert.Equal("a\n", resultOne);
    }
    
    [Fact]
    public void CACHE_EAST_OVERLAP_TXT()
    {
        var absoluteFilePath = new AbsoluteFilePath("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\PartiallyOverlappedChunks\\THE TELL-TALE HEART.txt",
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            absoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            Math.Ceiling(HeightOfEachRowInPixels) * (13),
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (20),
            Math.Ceiling(HeightOfEachRowInPixels) * (25));

        var resultOne = plainTextEditor.GetPlainText();

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (10) + _paddingInPixels,
            Math.Ceiling(HeightOfEachRowInPixels) * (16),
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (20),
            Math.Ceiling(HeightOfEachRowInPixels) * (3));

        var resultTwo = plainTextEditor.GetPlainText();

        Assert.Equal("a\n", resultOne);
    }

    [Fact]
    public void CACHE_SOUTH_OVERLAP_TXT()
    {
        var absoluteFilePath = new AbsoluteFilePath("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\PartiallyOverlappedChunks\\THE TELL-TALE HEART.txt",
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            absoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            Math.Ceiling(HeightOfEachRowInPixels) * (13),
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (20),
            Math.Ceiling(HeightOfEachRowInPixels) * (25));

        var resultOne = plainTextEditor.GetPlainText();

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            Math.Ceiling(HeightOfEachRowInPixels) * (20),
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (20),
            Math.Ceiling(HeightOfEachRowInPixels) * (25));

        var resultTwo = plainTextEditor.GetPlainText();

        Assert.Equal("a\n", resultOne);
    }

    [Fact]
    public void CACHE_WEST_OVERLAP_TXT()
    {
        var absoluteFilePath = new AbsoluteFilePath("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\PartiallyOverlappedChunks\\THE TELL-TALE HEART.txt",
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            absoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (15) + _paddingInPixels,
            Math.Ceiling(HeightOfEachRowInPixels) * (13),
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (20),
            Math.Ceiling(HeightOfEachRowInPixels) * (25));

        var resultOne = plainTextEditor.GetPlainText();

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            Math.Ceiling(HeightOfEachRowInPixels) * (13),
            0,
            0,
            Math.Ceiling(WidthOfEachCharacterInPixels) * (20),
            Math.Ceiling(HeightOfEachRowInPixels) * (25));

        var resultTwo = plainTextEditor.GetPlainText();

        Assert.Equal("a\n", resultOne);
    }

    public void DispatchMemoryMappedFileReadRequestAction(PlainTextEditorKey plainTextEditorKey,
        double scrollLeft,
        double scrollTop,
        double scrollWidth,
        double scrollHeight,
        double viewportWidth,
        double viewportHeight)
    {
        var paddedScrollLeft = Math.Max(0, scrollLeft - _paddingInPixels);
        var paddedScrollTop = Math.Max(0, scrollTop - _paddingInPixels);

        var paddedViewportWidth = viewportWidth + _paddingInPixels;
        var paddedViewportHeight = viewportHeight + _paddingInPixels;

        var virtualizeCoordinateSystemRequest = new VirtualizeCoordinateSystemRequest(
            paddedScrollLeft,
            paddedScrollTop,
            scrollWidth,
            scrollHeight,
            paddedViewportWidth,
            paddedViewportHeight,
            CancelTokenSourceAndGetNewToken());

        var virtualizeCoordinateSystemMessage = new VirtualizeCoordinateSystemMessage(
            virtualizeCoordinateSystemRequest, null);

        Dispatcher.Dispatch(new MemoryMappedFileReadRequestAction(plainTextEditorKey,
            virtualizeCoordinateSystemMessage));
    }

    public CancellationToken CancelTokenSourceAndGetNewToken()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        return _cancellationTokenSource.Token;
    }
}