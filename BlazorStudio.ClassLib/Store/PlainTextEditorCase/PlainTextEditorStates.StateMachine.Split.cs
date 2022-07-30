using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        // Used when cursor is within text and the 'Enter' key is pressed as an example. That token would get split into two separate tokens.
        public static PlainTextEditorRecord SplitCurrentToken(PlainTextEditorRecord focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit,
            bool isForced)
        {
            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();
            
            switch (currentToken.Kind)
            {
                case TextTokenKind.Default:
                    return SplitDefaultToken(focusedPlainTextEditorRecord, tokenToInsertBetweenSplit, isForced);
                case TextTokenKind.Whitespace:
                    return SplitWhitespaceToken(focusedPlainTextEditorRecord, tokenToInsertBetweenSplit, isForced);
                default:
                    return focusedPlainTextEditorRecord;
            }
        }
        
        public static PlainTextEditorRecord SplitDefaultToken(PlainTextEditorRecord focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit,
            bool isForced)
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
                var characterIndex = CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord)
                                     + focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;

                focusedPlainTextEditorRecord.FileHandle.Edit
                    .Insert(focusedPlainTextEditorRecord.CurrentRowIndex,
                        characterIndex,
                        tokenToInsertBetweenSplit.CopyText);
            }

            focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);
            
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var toBeChangedRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.List[toBeChangedRowIndex]);

            var toBeRemovedToken = toBeChangedRow.List[toBeRemovedTokenIndex];

            var nextRow = toBeChangedRow;

            int insertionOffset = 0;

            nextRow = nextRow with
            {
                List = nextRow.List
                    .Remove(toBeRemovedToken)
                    .Insert(rememberTokenIndex + insertionOffset++, tokenFirst),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            if (tokenToInsertBetweenSplit is not null)
            {
                nextRow = nextRow with
                {
                    List = nextRow.List
                        .Insert(rememberTokenIndex + insertionOffset++, tokenToInsertBetweenSplit),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            nextRow = nextRow with
            {
                List = nextRow.List
                    .Insert(rememberTokenIndex + insertionOffset++, tokenSecond),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.List.Replace(toBeChangedRow,
                nextRow);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex +
                    (tokenToInsertBetweenSplit is not null ? 2 : 1)
            };
        }

        public static PlainTextEditorRecord SplitWhitespaceToken(PlainTextEditorRecord focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit,
            bool isForced)
        {
            var rememberCurrentToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<WhitespaceTextToken>();

            if (rememberCurrentToken.WhitespaceKind != WhitespaceKind.Tab)
                return focusedPlainTextEditorRecord;

            var toBeRemovedTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;
            var toBeRemovedTokenIndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText;
            var toBeChangedRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            if (!isForced && tokenToInsertBetweenSplit is not null)
            {
                // Tab key so don't '+ focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value'
                int characterIndex = CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord);

                focusedPlainTextEditorRecord.FileHandle.Edit
                    .Remove(focusedPlainTextEditorRecord.CurrentRowIndex,
                        characterIndex,
                        characterCount: 1);
            }

            focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);
            
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var toBeChangedRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.List[toBeChangedRowIndex]);
            
            var toBeRemovedToken = toBeChangedRow.List[toBeRemovedTokenIndex];

            var nextRow = toBeChangedRow with
            {
                List = toBeChangedRow.List.Remove(toBeRemovedToken),
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
                var spaceWhiteSpaceToken = new WhitespaceTextToken(spaceKeyDownEventRecord)
                {
                    IndexInPlainText = null
                };

                nextRow = nextRow with
                {
                    List = nextRow.List.Insert(toBeRemovedTokenIndex + i, spaceWhiteSpaceToken),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            if (tokenToInsertBetweenSplit is not null)
            {
                nextRow = nextRow with
                {
                    List = nextRow.List
                        .Insert(toBeRemovedTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 1,
                            tokenToInsertBetweenSplit),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            var nextRowList = focusedPlainTextEditorRecord.List
                .Replace(toBeChangedRow, nextRow);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = toBeRemovedTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 
                                    (tokenToInsertBetweenSplit is not null ? 1 : 0)
            };
        }
    }
}
