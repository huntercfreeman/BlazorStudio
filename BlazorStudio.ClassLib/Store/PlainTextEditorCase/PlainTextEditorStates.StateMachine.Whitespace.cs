using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static async Task<PlainTextEditorRecordBase> HandleWhitespaceAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            var rememberToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<TextTokenBase>();

            if (rememberToken.IndexInPlainText!.Value != rememberToken.PlainText.Length - 1)
            {
                if (KeyboardKeyFacts.NewLineCodes.ALL_NEW_LINE_CODES.Contains(keyDownEventRecord.Code))
                {
                    focusedPlainTextEditorRecord = await SplitCurrentTokenAsync(
                        focusedPlainTextEditorRecord,
                        null,
                        keyDownEventRecord.IsForced,
                        cancellationToken
                    );

                    return await InsertNewLineAsync(focusedPlainTextEditorRecord,
                        keyDownEventRecord,
                        cancellationToken);
                }

                return await SplitCurrentTokenAsync(
                    focusedPlainTextEditorRecord,
                        new WhitespaceTextToken(keyDownEventRecord),
                    keyDownEventRecord.IsForced,
                    cancellationToken
                );
            }
            else
            {
                if (KeyboardKeyFacts.NewLineCodes.ALL_NEW_LINE_CODES.Contains(keyDownEventRecord.Code))
                {
                    return await InsertNewLineAsync(focusedPlainTextEditorRecord,
                        keyDownEventRecord,
                        cancellationToken);
                }

                var whitespaceToken = new WhitespaceTextToken(keyDownEventRecord);

                if (!keyDownEventRecord.IsForced)
                {
                    var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                                             true,
                                             cancellationToken)
                                         + focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;

                    await focusedPlainTextEditorRecord.FileHandle.Edit
                        .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                            characterIndex,
                            whitespaceToken.CopyText,
                            cancellationToken);
                }

                return await InsertNewCurrentTokenAfterCurrentPositionAsync(focusedPlainTextEditorRecord,
                    whitespaceToken,
                    cancellationToken);
            }
        }
    }
}
