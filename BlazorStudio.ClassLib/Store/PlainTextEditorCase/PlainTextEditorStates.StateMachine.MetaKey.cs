using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static PlainTextEditorRecord HandleMetaKey(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MetaKeys.BACKSPACE_KEY:
                    return HandleBackspaceKey(focusedPlainTextEditorRecord, keyDownEventRecord);
                default:
                   return focusedPlainTextEditorRecord;
            }
        }

        public static PlainTextEditorRecord HandleBackspaceKey(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Default)
            {
                // Remove character from word

                return HandleDefaultBackspace(focusedPlainTextEditorRecord, keyDownEventRecord);
            }

            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.StartOfRow &&
                focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>().Tokens.Count > 1)
            {
                // Remove newline character

                // TODO: 'focusedPlainTextEditorRecord.CurrentRowIndex != 0' seems incorrect but I am uncertain. It was added due to indexing into an array at -1
                if (!keyDownEventRecord.IsForced && focusedPlainTextEditorRecord.CurrentRowIndex != 0)
                {
                    var previousRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex - 1;
                    var previousRow = focusedPlainTextEditorRecord.Rows[previousRowIndex];

                    var characterIndexTotal = 0;

                    foreach (var token in previousRow.Tokens)
                    {
                        characterIndexTotal += token.CopyText.Length;
                    }

                    focusedPlainTextEditorRecord.FileHandle.Edit
                        .RemoveAsync(previousRowIndex,
                            characterIndexTotal - 1,
                            characterCount: 1);
                }

                focusedPlainTextEditorRecord = MoveCurrentRowToEndOfPreviousRow(focusedPlainTextEditorRecord);
            }
            else
            {
                // Remove non word token (perhaps whitespace)

                // TODO: 'focusedPlainTextEditorRecord.CurrentRowIndex != 0' seems incorrect but I am uncertain. It was added due to indexing into an array at -1
                if (!keyDownEventRecord.IsForced && focusedPlainTextEditorRecord.CurrentRowIndex != 0)
                {
                    if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.StartOfRow)
                    {
                        var previousRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex - 1;
                        var previousRow = focusedPlainTextEditorRecord.Rows[previousRowIndex];

                        var characterIndexTotal = 0;

                        foreach (var token in previousRow.Tokens)
                        {
                            characterIndexTotal += token.CopyText.Length;
                        }

                        focusedPlainTextEditorRecord.FileHandle.Edit
                            .RemoveAsync(previousRowIndex,
                                characterIndexTotal - 1,
                                characterCount: 1);
                    }
                    else
                    {
                        var characterIndex = CalculateCurrentTokenStartingCharacterIndexRespectiveToRow(focusedPlainTextEditorRecord)
                                             + focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;

                        focusedPlainTextEditorRecord.FileHandle.Edit
                            .RemoveAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                                characterIndex - 1,
                                characterCount: 1);
                    }
                }

                focusedPlainTextEditorRecord = RemoveCurrentToken(focusedPlainTextEditorRecord);
            }

            focusedPlainTextEditorRecord = MergeTokensIfApplicable(focusedPlainTextEditorRecord);

            return focusedPlainTextEditorRecord;
        }
    }
}
