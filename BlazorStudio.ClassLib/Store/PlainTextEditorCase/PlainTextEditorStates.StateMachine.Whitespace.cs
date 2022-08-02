using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static PlainTextEditorRecord HandleWhitespace(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            var rememberToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<TextTokenBase>();

            if (rememberToken.IndexInPlainText!.Value != rememberToken.PlainText.Length - 1)
            {
                if (KeyboardKeyFacts.NewLineCodes.ALL_NEW_LINE_CODES.Contains(keyDownEventRecord.Code))
                {
                    focusedPlainTextEditorRecord = SplitCurrentToken(
                        focusedPlainTextEditorRecord,
                        null,
                        keyDownEventRecord.IsForced
                    );

                    return InsertNewLine(focusedPlainTextEditorRecord,
                        keyDownEventRecord);
                }

                return SplitCurrentToken(
                    focusedPlainTextEditorRecord,
                        new WhitespaceTextToken(keyDownEventRecord),
                    keyDownEventRecord.IsForced
                );
            }
            else
            {
                if (KeyboardKeyFacts.NewLineCodes.ALL_NEW_LINE_CODES.Contains(keyDownEventRecord.Code))
                {
                    return InsertNewLine(focusedPlainTextEditorRecord,
                        keyDownEventRecord);
                }

                var whitespaceToken = new WhitespaceTextToken(keyDownEventRecord);

                if (!keyDownEventRecord.IsForced)
                {
                    var characterIndex = CalculateCurrentTokenStartingCharacterIndexRespectiveToRow(focusedPlainTextEditorRecord)
                                         + focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;

                    focusedPlainTextEditorRecord.FileHandle.Edit
                        .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                            characterIndex,
                            whitespaceToken.CopyText);
                }

                return InsertNewCurrentTokenAfterCurrentPosition(focusedPlainTextEditorRecord,
                    whitespaceToken);
            }
        }
    }
}
