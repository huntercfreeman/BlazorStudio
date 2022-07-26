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
    public void MEMORY_MAPPED_FILE_IS_EQUAL_TO_STREAM_READER()
    {
        var absoluteFilePath = new AbsoluteFilePath("C:\\Users\\hunte\\source\\BlazorStudio\\BlazorStudio.Tests\\TestData\\Hamlet_ Entire Play.html",
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

        DispatchMemoryMappedFileReadRequestAction(plainTextEditorKey,
            1000,
            1000,
            0,
            0,
            1500,
            1500);
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