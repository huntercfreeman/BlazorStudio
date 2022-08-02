using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static async Task<PlainTextEditorRecord> HandleMetaKeyAsync(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MetaKeys.BACKSPACE_KEY:
                    return await HandleBackspaceKeyAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                default:
                   return focusedPlainTextEditorRecord;
            }
        }

        public static async Task<PlainTextEditorRecord> HandleBackspaceKeyAsync(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Default)
            {
                // Remove character from word

                return await HandleDefaultBackspaceAsync(focusedPlainTextEditorRecord, 
                    keyDownEventRecord,
                    cancellationToken);
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

                    await focusedPlainTextEditorRecord.FileHandle.Edit
                        .RemoveAsync(previousRowIndex,
                            characterIndexTotal - 1,
                            cancellationToken,
                            characterCount: 1);
                }

                focusedPlainTextEditorRecord = await MoveCurrentRowToEndOfPreviousRowAsync(focusedPlainTextEditorRecord,
                    cancellationToken);
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

                        await focusedPlainTextEditorRecord.FileHandle.Edit
                            .RemoveAsync(previousRowIndex,
                                characterIndexTotal - 1,
                                cancellationToken,
                                characterCount: 1);
                    }
                    else
                    {
                        var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                                                 cancellationToken)
                                             + focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;

                        await focusedPlainTextEditorRecord.FileHandle.Edit
                            .RemoveAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                                characterIndex - 1,
                                cancellationToken,
                                characterCount: 1);
                    }
                }

                focusedPlainTextEditorRecord = await RemoveCurrentTokenAsync(focusedPlainTextEditorRecord,
                    cancellationToken);
            }

            focusedPlainTextEditorRecord = await MergeTokensIfApplicableAsync(focusedPlainTextEditorRecord,
                cancellationToken);

            return focusedPlainTextEditorRecord;
        }
    }
}
