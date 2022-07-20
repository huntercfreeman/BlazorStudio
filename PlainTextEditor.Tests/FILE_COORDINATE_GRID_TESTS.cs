using BlazorStudio.Shared.FileSystem.Classes;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.Tests;

public class FILE_COORDINATE_GRID_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Fact]
    public async Task INITIALIZE_ASYNC_FILE_COORDINATE_GRID()
    {
        var absoluteFilePathString = "C:\\Users\\hunte\\source\\BlazorStudio\\PlainTextEditor.Tests\\TestData\\helloWorld_NEW-LINE.c";

        var absoluteFilePath = new AbsoluteFilePath(absoluteFilePathString, false);

        var fileCoordinateGrid = await FileCoordinateGridFactory
            .ConstructFileCoordinateGridAsync(absoluteFilePath);
    }
}