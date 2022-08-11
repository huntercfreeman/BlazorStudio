using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        // Used when cursor is within text and the 'Enter' key is pressed as an example. That token would get split into two separate tokens.
        public static async Task<PlainTextEditorRecordBase> SplitCurrentTokenAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit,
            bool isForced,
            CancellationToken cancellationToken)
        {
            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();
            
            switch (currentToken.Kind)
            {
                case TextTokenKind.Default:
                    return await SplitDefaultTokenAsync(focusedPlainTextEditorRecord, 
                        tokenToInsertBetweenSplit, 
                        isForced,
                        cancellationToken);
                case TextTokenKind.Whitespace:
                    return await SplitWhitespaceTokenAsync(focusedPlainTextEditorRecord, 
                        tokenToInsertBetweenSplit, 
                        isForced,
                        cancellationToken);
                default:
                    return focusedPlainTextEditorRecord;
            }
        }
        
        public static async Task<PlainTextEditorRecordBase> SplitDefaultTokenAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit,
            bool isForced,
            CancellationToken cancellationToken)
        {
            var rememberCurrentToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<DefaultTextToken>();

            var rememberTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;

            var firstSplitContent = rememberCurrentToken.Content
                .Substring(0, rememberCurrentToken.IndexInPlainText!.Value + 1);

            var secondSplitContent = rememberCurrentToken.Content
                    .Substring(rememberCurrentToken.IndexInPlainText!.Value + 1);

            int? tokenFirstIndexInContent = tokenToInsertBetweenSplit is null
                ? firstSplitContent.Length - 1
                : null;

            var tokenFirst = new DefaultTextToken()
            {
                Content = firstSplitContent,
                IndexInPlainText = tokenFirstIndexInContent
            };
            
            var tokenSecond = new DefaultTextToken()
            {
                Content = secondSplitContent
            };

            var toBeRemovedTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;
            var toBeChangedRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            if (!isForced && tokenToInsertBetweenSplit is not null)
            {
                var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                                         true,
                                         cancellationToken)
                                     + focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;

                await focusedPlainTextEditorRecord.FileHandle.Edit
                    .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                        characterIndex,
                        tokenToInsertBetweenSplit.CopyText,
                        cancellationToken);
            }

            focusedPlainTextEditorRecord = await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                cancellationToken);
            
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);

            var toBeChangedRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.Rows[toBeChangedRowIndex]);

            var toBeRemovedToken = toBeChangedRow.Tokens[toBeRemovedTokenIndex];

            var nextRow = toBeChangedRow;

            int insertionOffset = 0;

            nextRow = nextRow with
            {
                Tokens = nextRow.Tokens
                    .Remove(toBeRemovedToken)
                    .Insert(rememberTokenIndex + insertionOffset++, tokenFirst),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            if (tokenToInsertBetweenSplit is not null)
            {
                nextRow = nextRow with
                {
                    Tokens = nextRow.Tokens
                        .Insert(rememberTokenIndex + insertionOffset++, tokenToInsertBetweenSplit),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            nextRow = nextRow with
            {
                Tokens = nextRow.Tokens
                    .Insert(rememberTokenIndex + insertionOffset++, tokenSecond),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.Rows.Replace(toBeChangedRow,
                nextRow);

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList,
                CurrentTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex +
                    (tokenToInsertBetweenSplit is not null ? 2 : 1)
            };
        }

        public static async Task<PlainTextEditorRecordBase> SplitWhitespaceTokenAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit,
            bool isForced,
            CancellationToken cancellationToken)
        {
            var rememberCurrentToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<WhitespaceTextToken>();

            if (rememberCurrentToken.WhitespaceKind != WhitespaceKind.Tab)
                return focusedPlainTextEditorRecord;

            var toBeRemovedTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;
            var toBeRemovedTokenIndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText;
            var toBeChangedRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            if (!isForced)
            {
                // Tab key so don't '+ focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value'
                int characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                    true,
                    cancellationToken);

                var spaceCount = tokenToInsertBetweenSplit is null
                    ? 4  // newline key
                    : 5; // space key

                await focusedPlainTextEditorRecord.FileHandle.Edit
                    .RemoveAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                        characterIndex - 1,
                        cancellationToken,
                        characterCount: 1);

                await focusedPlainTextEditorRecord.FileHandle.Edit
                    .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                        characterIndex - 1,
                        new string(' ', spaceCount),
                        cancellationToken);
            }

            focusedPlainTextEditorRecord = await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                cancellationToken);
            
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);

            var toBeChangedRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.Rows[toBeChangedRowIndex]);
            
            var toBeRemovedToken = toBeChangedRow.Tokens[toBeRemovedTokenIndex];

            var nextRow = toBeChangedRow with
            {
                Tokens = toBeChangedRow.Tokens.Remove(toBeRemovedToken),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var spaceKeyDownEventRecord = new KeyDownEventRecord(
                KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                false,
                false,
                false
            );

            for (int i = 0; i < 4; i++)
            {
                int? indexInPlainText = null;

                // if newline
                if (tokenToInsertBetweenSplit is null)
                {
                    indexInPlainText = i == toBeRemovedTokenIndexInPlainText
                        ? 0
                        : null;
                }

                var spaceWhiteSpaceToken = new WhitespaceTextToken(spaceKeyDownEventRecord)
                {
                    IndexInPlainText = indexInPlainText
                };

                nextRow = nextRow with
                {
                    Tokens = nextRow.Tokens.Insert(toBeRemovedTokenIndex + i, spaceWhiteSpaceToken),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            if (tokenToInsertBetweenSplit is not null)
            {
                nextRow = nextRow with
                {
                    Tokens = nextRow.Tokens
                        .Insert(toBeRemovedTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 1,
                            tokenToInsertBetweenSplit),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            var nextRowList = focusedPlainTextEditorRecord.Rows
                .Replace(toBeChangedRow, nextRow);

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList,
                CurrentTokenIndex = toBeRemovedTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 
                                    (tokenToInsertBetweenSplit is not null ? 1 : 0)
            };
        }
    }
}
