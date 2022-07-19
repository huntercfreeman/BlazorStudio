using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.Tests;

public class INSERTION_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    private bool asyncEffectFinished;

    [Fact]
    public async Task HELLO_WORLD_C()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        string helloWorldRelativeFilePath = "./TestData/helloWorld.c";

        var expectedContent = await File
            .ReadAllTextAsync(helloWorldRelativeFilePath);

        Dispatcher.Dispatch(new PlainTextEditorInitializeAction(plainTextEditorKey,
            helloWorldRelativeFilePath));

        State.StateChanged += StateOnStateChanged;

        while (true)
        {
            if (asyncEffectFinished)
                break;

            await Task.Delay(500);
        }

        var documentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(expectedContent, documentPlainText);

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }
    
    [Fact]
    public async Task BlazorStudioSolution()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        string blazorStudioSlnRelativeFilePath = "./TestData/TEST_BlazorStudio.sln";

        var expectedContent = await File
            .ReadAllTextAsync(blazorStudioSlnRelativeFilePath);

        Dispatcher.Dispatch(new PlainTextEditorInitializeAction(plainTextEditorKey,
            blazorStudioSlnRelativeFilePath));

        State.StateChanged += StateOnStateChanged;

        while (true)
        {
            if (asyncEffectFinished)
                break;

            await Task.Delay(500);
        }

        var documentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(expectedContent, documentPlainText);

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }
    
    // TODO: This never finishes when I run it. I need to fix this. It appears to be an optimization issue of speed. Perhaps streaming file content instead of reading a massive lump of text into memory is in order at this point.
    //[Fact]
    //public async Task HamletEntirePlayHtml()
    //{
    //    var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
    //    Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

    //    string blazorStudioSlnRelativeFilePath = "./TestData/Hamlet_ Entire Play.html";

    //    var expectedContent = await File
    //        .ReadAllTextAsync(blazorStudioSlnRelativeFilePath);

    //    Dispatcher.Dispatch(new PlainTextEditorInitializeAction(plainTextEditorKey,
    //        blazorStudioSlnRelativeFilePath));

    //    State.StateChanged += StateOnStateChanged;

    //    while (true)
    //    {
    //        if (asyncEffectFinished)
    //            break;

    //        await Task.Delay(500);
    //    }

    //    var documentPlainText = State.Value.Map[plainTextEditorKey]
    //        .GetPlainText();

    //    Assert.Equal(expectedContent, documentPlainText);

    //    Assert.Single(State.Value.Map);
    //    Assert.Single(State.Value.Array);
    //}

    private void StateOnStateChanged(object? sender, EventArgs e)
    {
        asyncEffectFinished = true;
    }
}