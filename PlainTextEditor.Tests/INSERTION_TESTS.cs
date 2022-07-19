using PlainTextEditor.ClassLib.Keyboard;
using PlainTextEditor.ClassLib.Store.KeyDownEventCase;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.Tests;

public class INSERTION_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    [Theory]
    [InlineData("./TestData/helloWorld_NEW-LINE.c")]
    [InlineData("./TestData/helloWorld_CARRIAGE-RETURN-NEW-LINE.c")]
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

    [Theory]
    [InlineData("./TestData/helloWorld_NEW-LINE.c", "\n")]
    [InlineData("./TestData/helloWorld_CARRIAGE-RETURN-NEW-LINE.c", "\r\n")]
    public async Task CARRIAGE_RETURN_MATCHING_ON_NEW_LINE(string relativeFilePath,
        string expectedAdditionToText)
    {
        var keyboardEvent = new KeyDownEventRecord(KeyboardKeyFacts.NewLineCodes.ENTER_CODE,
            KeyboardKeyFacts.NewLineCodes.ENTER_CODE,
            false,
            false,
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructPlainTextEditorRecordAction(plainTextEditorKey));

        var expectedContent = await File
            .ReadAllTextAsync(relativeFilePath);

        await DispatchHelperAsync(
            new PlainTextEditorInitializeAction(plainTextEditorKey, relativeFilePath),
            State);

        var documentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(expectedContent, documentPlainText);

        var keyboardEventExpectedContent = expectedContent + expectedAdditionToText;

        await DispatchHelperAsync(
            new KeyDownEventAction(plainTextEditorKey, keyboardEvent),
            State);

        var newLineEventDocumentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(keyboardEventExpectedContent, newLineEventDocumentPlainText);

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }
}