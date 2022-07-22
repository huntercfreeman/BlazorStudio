using BlazorStudio.Shared.FileSystem.Classes;
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
    public async Task READ_FILE_INTO_EDITOR(string relativeFilePathString)
    {
        var relativeFilePath = new RelativeFilePath(relativeFilePathString, false);

        var absoluteFilePath = new AbsoluteFilePath(CurrentDirectoryAbsoluteFilePath, relativeFilePath);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructInMemoryPlainTextEditorRecordAction(plainTextEditorKey));

        var expectedContent = await File
            .ReadAllTextAsync(absoluteFilePath.GetAbsoluteFilePathString());

        await DispatchHelperAsync(
            new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey, absoluteFilePath),
            State);

        var documentPlainText = State.Value.Map[plainTextEditorKey]
            .GetPlainText();

        Assert.Equal(expectedContent, documentPlainText);

        Assert.Single(State.Value.Map);
        Assert.Single(State.Value.Array);
    }

    [Theory]
    [InlineData("./TestData/helloWorld_NEW-LINE.c", "\n")]
    [InlineData("./TestData/helloWorld_CARRIAGE-RETURN-NEW-LINE.c", "\r\n")]
    public async Task CARRIAGE_RETURN_MATCHING_ON_NEW_LINE(string relativeFilePathString,
        string expectedAdditionToText)
    {
        var relativeFilePath = new RelativeFilePath(relativeFilePathString, false);

        var absoluteFilePath = new AbsoluteFilePath(CurrentDirectoryAbsoluteFilePath, relativeFilePath);

        var keyboardEvent = new KeyDownEventRecord(KeyboardKeyFacts.NewLineCodes.ENTER_CODE,
            KeyboardKeyFacts.NewLineCodes.ENTER_CODE,
            false,
            false,
            false);

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        Dispatcher.Dispatch(new ConstructInMemoryPlainTextEditorRecordAction(plainTextEditorKey));

        var expectedContent = await File
            .ReadAllTextAsync(absoluteFilePath.GetAbsoluteFilePathString());

        await DispatchHelperAsync(
            new ConstructMemoryMappedFilePlainTextEditorRecordAction(plainTextEditorKey, absoluteFilePath),
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