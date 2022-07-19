using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.Tests;

public class INSERTION_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Theory]
    [InlineData("./TestData/helloWorld.c")]
    [InlineData("./TestData/TEST_BlazorStudio.sln")]
    public async Task READ_FILE_INTO_EDITOR(string relativeFilePath)
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        var expectedContent = await File
            .ReadAllTextAsync(relativeFilePath);

        await DispatchHelperAsync(
            new PlainTextEditorInitializeAction(plainTextEditorKey, relativeFilePath),
            State);

        Dispatcher.Dispatch(new PlainTextEditorInitializeAction(plainTextEditorKey,
            relativeFilePath));

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

    //    State.StateChanged += StateOnStateChanged;

    //    Dispatcher.Dispatch(new PlainTextEditorInitializeAction(plainTextEditorKey,
    //        blazorStudioSlnRelativeFilePath));


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

    [Fact]
    public async Task SHOULD_NEW_LINE()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        string helloWorldRelativeFilePath = "./TestData/helloWorld.c";

        var expectedContent = await File
            .ReadAllTextAsync(helloWorldRelativeFilePath);

        await DispatchHelperAsync(
            new PlainTextEditorInitializeAction(plainTextEditorKey, helloWorldRelativeFilePath),
            State);

        var documentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(expectedContent, documentPlainText);

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }

    [Fact]
    public async Task SHOULD_CARRIAGE_RETURN_NEW_LINE()
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        string helloWorldRelativeFilePath = "./TestData/helloWorld.c";

        var expectedContent = await File
            .ReadAllTextAsync(helloWorldRelativeFilePath);

        await DispatchHelperAsync(
            new PlainTextEditorInitializeAction(plainTextEditorKey, helloWorldRelativeFilePath),
            State);

        var documentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(expectedContent, documentPlainText);

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }
}