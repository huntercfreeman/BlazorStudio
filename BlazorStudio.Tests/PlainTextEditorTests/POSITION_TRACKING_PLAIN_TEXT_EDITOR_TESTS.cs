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
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();
        
        // Generate input
        {
            var builder = new StringBuilder();

            for (int i = 0; i < muliplyInput; i++)
            {
                builder.Append(input);
            }

            input = builder.ToString();
        }

        // Initialize
        {
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
        }

        // Ensure editor is empty
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];

            Assert.Equal(string.Empty, editor.GetDocumentPlainText());

            var expectedUseCarriageReturnNewLine = false;
            Assert.Equal(expectedUseCarriageReturnNewLine, editor.UseCarriageReturnNewLine);

            var expectedPositionIndex = 0;
            Assert.Equal(expectedPositionIndex, editor.CurrentPositionIndex);
        }

        // Insert
        {
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
        }

        // Assert
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];

            // The editor needs to be tested with "\r\n"
            // however the expectation is that they're treated as "\r\n".Length == 1
            // this line replaces "\r\n" then "\r" purely to get the expected value to assert
            // for tracking CurrentPositionIndex.
            input = input
                .Replace("\r\n", "\n")
                .Replace("\r", string.Empty);

            var expectedPositionIndex = input.Length;
            Assert.Equal(expectedPositionIndex, editor.CurrentPositionIndex);
        }
    }
    
    /// <summary>
    /// As I move around a file will my position index change correctly.
    /// </summary>
    [Theory]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/EmptyFile.txt", 0)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/NewLine.txt", 0)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/CarriageReturnNewLine.txt", 0)]
    public async Task TRACK_ARROW_LEFT_MOVEMENT(string absoluteFilePathString, int positionAfterArrowLeft)
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        // Initialize
        {
            var absoluteFilePath = new AbsoluteFilePath(absoluteFilePathString, false);

            var fileSystemProvider = new FileSystemProvider();

            await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    absoluteFilePath,
                    fileSystemProvider,
                    CancellationToken.None),
                PlainTextEditorStateWrap);

            Assert.Single(PlainTextEditorStateWrap.Value.Map);
            Assert.Single(PlainTextEditorStateWrap.Value.Array);
        }

        // Move
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
            var beforeMovementPositionIndex = editor.CurrentPositionIndex;

            var keyDownEventRecord = new KeyDownEventRecord(
                KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY,
                KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY,
                false,
                false,
                false
            );
            
            var action = new KeyDownEventAction(plainTextEditorKey, keyDownEventRecord, CancellationToken.None);
            
            await DispatchHelperAsync(action, PlainTextEditorStateWrap);
        }

        // Assert
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];

            Assert.Equal(positionAfterArrowLeft, editor.CurrentPositionIndex);
        }
    }
    
    /// <summary>
    /// As I move around a file will my position index change correctly.
    /// </summary>
    [Theory]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/EmptyFile.txt", 0)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/NewLine.txt", 1)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/CarriageReturnNewLine.txt", 2)]
    public async Task TRACK_ARROW_DOWN_MOVEMENT(string absoluteFilePathString, int positionAfterArrowDown)
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        // Initialize
        {
            var absoluteFilePath = new AbsoluteFilePath(absoluteFilePathString, false);

            var fileSystemProvider = new FileSystemProvider();

            await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    absoluteFilePath,
                    fileSystemProvider,
                    CancellationToken.None),
                PlainTextEditorStateWrap);
            
            Assert.Single(PlainTextEditorStateWrap.Value.Map);
            Assert.Single(PlainTextEditorStateWrap.Value.Array);
        }

        // Move
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
            var beforeMovementPositionIndex = editor.CurrentPositionIndex;

            var keyDownEventRecord = new KeyDownEventRecord(
                KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                false,
                false,
                false
            );
            
            var action = new KeyDownEventAction(plainTextEditorKey, keyDownEventRecord, CancellationToken.None);
            
            await DispatchHelperAsync(action, PlainTextEditorStateWrap);
        }

        // Assert
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
            
            Assert.Equal(positionAfterArrowDown, editor.CurrentPositionIndex);
        }
    }
    
    /// <summary>
    /// As I move around a file will my position index change correctly.
    /// </summary>
    [Theory]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/EmptyFile.txt", 0)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/NewLine.txt", 0)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/CarriageReturnNewLine.txt", 0)]
    public async Task TRACK_ARROW_UP_MOVEMENT(string absoluteFilePathString, int positionAfterArrowUp)
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        // Initialize
        {
            var absoluteFilePath = new AbsoluteFilePath(absoluteFilePathString, false);

            var fileSystemProvider = new FileSystemProvider();

            await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    absoluteFilePath,
                    fileSystemProvider,
                    CancellationToken.None),
                PlainTextEditorStateWrap);

            Assert.Single(PlainTextEditorStateWrap.Value.Map);
            Assert.Single(PlainTextEditorStateWrap.Value.Array);
        }

        // Move
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
            var beforeMovementPositionIndex = editor.CurrentPositionIndex;
            
            var keyDownEventRecord = new KeyDownEventRecord(
                KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY,
                KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY,
                false,
                false,
                false
            );
            
            var action = new KeyDownEventAction(plainTextEditorKey, keyDownEventRecord, CancellationToken.None);
            
            await DispatchHelperAsync(action, PlainTextEditorStateWrap);
        }
        
        // Assert
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];

            Assert.Equal(positionAfterArrowUp, editor.CurrentPositionIndex);
        }
    }
    
    /// <summary>
    /// As I move around a file will my position index change correctly.
    /// </summary>
    [Theory]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/EmptyFile.txt", 0)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/NewLine.txt", 1)]
    [InlineData("/home/hunter/Repos/BlazorStudio/BlazorStudio.Tests/TestData/CarriageReturnNewLine.txt", 1)]
    public async Task TRACK_ARROW_RIGHT_MOVEMENT(string absoluteFilePathString, int positionAfterArrowRight)
    {
        var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        // Initialize
        {
            var absoluteFilePath = new AbsoluteFilePath(absoluteFilePathString, false);

            var fileSystemProvider = new FileSystemProvider();

            await DispatchHelperAsync(new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    absoluteFilePath,
                    fileSystemProvider,
                    CancellationToken.None),
                PlainTextEditorStateWrap);

            Assert.Single(PlainTextEditorStateWrap.Value.Map);
            Assert.Single(PlainTextEditorStateWrap.Value.Array);
        }

        // Move
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];
            var beforeMovementPositionIndex = editor.CurrentPositionIndex;
            
            var keyDownEventRecord = new KeyDownEventRecord(
                KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY,
                KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY,
                false,
                false,
                false
            );
            
            var action = new KeyDownEventAction(plainTextEditorKey, keyDownEventRecord, CancellationToken.None);
            
            await DispatchHelperAsync(action, PlainTextEditorStateWrap);
        }
        
        // Assert
        {
            var editor = PlainTextEditorStateWrap.Value.Map[plainTextEditorKey];

            Assert.Equal(positionAfterArrowRight, editor.CurrentPositionIndex);
        }
    }
}