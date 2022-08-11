using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;

namespace BlazorStudio.Tests.PlainTextEditorTests;

/// <summary>
/// In this file I use absolute file paths "string absoluteFilePathString" as parameters
/// for tests. This is not ideal as the tests will fail
/// if someone other than I runs the tests. But I do not want to take time
/// writing logic for relative paths at this moment in time.
/// </summary>
public class POSITION_TRACKING_PLAIN_TEXT_EDITOR_TESTS : PLAIN_TEXT_EDITOR_STATES_TESTS
{
    /// <summary>
    /// As I type 'abcdefg' will my position index increment correctly.
    /// </summary>
    [Theory]
    [InlineData("abcdefg", 1)]
    [InlineData("\n", 1)]
    [InlineData("\r\n", 1)]
    [InlineData("\nab\r\ncd\refg", 371)]
    [InlineData("\n", 371)]
    [InlineData("\r\n", 371)]
    public async Task TRACK_TEXT_INSERTION(string input, int muliplyInput)
    {
        var builder = new StringBuilder();

        for (int i = 0; i < muliplyInput; i++)
        {
            builder.Append(input);
        }

        input = builder.ToString();

        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        var absoluteFilePath = 
            new AbsoluteFilePath("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/PositionTracking.txt",
                false);

        var fileSystemProvider = new FileSystemProvider();
        
        await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey, 
                absoluteFilePath, 
                fileSystemProvider, 
                CancellationToken.None),
            PlainTextEditorStateWrap);
        
        Assert.Single(PlainTextEditorStateWrap.Value.Map);
        Assert.Single(PlainTextEditorStateWrap.Value.Array);

        var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
        
        Assert.Equal(string.Empty, editor.GetPlainText());

        var expectedUseCarriageReturnNewLine = false;
        Assert.Equal(expectedUseCarriageReturnNewLine, editor.UseCarriageReturnNewLine);

        var expectedPositionIndex = 0;
        Assert.Equal(expectedPositionIndex, editor.CurrentPositionIndex);

        foreach (var character in input)
        {
            var previousCharacterWasCarriageReturn = false;

            string MutateIfPreviousCharacterWasCarriageReturn()
            {
                return previousCharacterWasCarriageReturn
                    ? KeyboardKeyFacts.WhitespaceKeys.CARRIAGE_RETURN_NEW_LINE_CODE
                    : KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE;
            }

            if (character == '\r')
            {
                previousCharacterWasCarriageReturn = true;
                continue;
            }
            
            var code = character switch
            {
                '\t' => KeyboardKeyFacts.WhitespaceKeys.TAB_CODE,
                ' ' => KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                '\n' => MutateIfPreviousCharacterWasCarriageReturn(),
                _ => character.ToString()
            };

            previousCharacterWasCarriageReturn = false;

            var keyDownRecord = new KeyDownEventRecord(
                character.ToString(),
                code,
                false,
                false,
                false
            );
            
            var action = new KeyDownEventAction(plainTextEditorKey, keyDownRecord, CancellationToken.None);
            
            await DispatchHelperAsync(action, PlainTextEditorStateWrap);
        }

        editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
        
        // The editor needs to be tested with "\r\n"
        // however the expectation is that they're treated as "\r\n".Length == 1
        // this line replaces "\r\n" then "\r" purely to get the expected value to assert
        // for tracking CurrentPositionIndex.
        input = input
            .Replace("\r\n", "\n")
            .Replace("\r", string.Empty);
        
        expectedPositionIndex = input.Length;
        Assert.Equal(expectedPositionIndex, editor.CurrentPositionIndex);
    }
}