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
        var absoluteFilePath = new AbsoluteFilePath("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\CacheTests\\SingleCharacter.txt",
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey,
            absoluteFilePath));

        var plainTextEditor = State.Value.Map[plainTextEditorKey];

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            0,
            0,
            0,
            0,
            500,
            500);

        var resultOne = plainTextEditor.GetPlainText();

        Assert.Equal("a\n", resultOne);

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

        var resultTwo = plainTextEditor.GetPlainText();

        Assert.Equal(1, plainTextEditor.CacheCount);

        Assert.Equal("a\n", resultTwo);
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
            250,
            500,
            0,
            0,
            500,
            500);

        var resultOne = plainTextEditor.GetPlainText();

        Assert.Equal("a\n", resultOne);

        //for (int i = 0; i < 10; i++)
        //{
        //    DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
        //        0,
        //        0,
        //        0,
        //        0,
        //        Math.Ceiling(WidthOfEachCharacterInPixels) * (i),
        //        Math.Ceiling(HeightOfEachRowInPixels) * (i));
        //}

        //var resultTwo = plainTextEditor.GetPlainText();

        //Assert.Equal(1, plainTextEditor.CacheCount);

        //Assert.Equal("a\n", resultTwo);
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