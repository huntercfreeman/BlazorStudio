using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static async Task<PlainTextEditorRecordBase> HandleMovementAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT_KEY:
                    return await HandleArrowLeftAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN_KEY:
                    return await HandleArrowDownAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP_KEY:
                    return await HandleArrowUpAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT_KEY:
                    return await HandleArrowRightAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                case KeyboardKeyFacts.MovementKeys.HOME_KEY:
                    return await HandleHomeAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                case KeyboardKeyFacts.MovementKeys.END_KEY:
                    return await HandleEndAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
            }

            return focusedPlainTextEditorRecord;
        }

        public static async Task<PlainTextEditorRecordBase> HandleArrowLeftAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            if (keyDownEventRecord.CtrlWasPressed)
            {
                var rememberTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;
                var rememberTokenWasWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                focusedPlainTextEditorRecord = await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord, 
                    cancellationToken);

                var currentTokenIsWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                if ((rememberTokenWasWhitespace && currentTokenIsWhitespace) &&
                    (rememberTokenKey != focusedPlainTextEditorRecord.CurrentTextTokenKey))
                {
                    return await HandleMovementAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                }

                return focusedPlainTextEditorRecord;
            }

            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            if (currentToken.IndexInPlainText == 0)
            {
                return await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                    cancellationToken);
            }
            else
            {
                var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = currentToken.IndexInPlainText - 1
                };

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex - 1,
                    CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex - 1
                };

                focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                        replacementCurrentToken,
                        cancellationToken);
            }

            return focusedPlainTextEditorRecord;
        }

        public static async Task<PlainTextEditorRecordBase> HandleArrowDownAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            if (focusedPlainTextEditorRecord.CurrentRowIndex >=
                focusedPlainTextEditorRecord.Rows.Count - 1)
            {
                return focusedPlainTextEditorRecord;
            }

            var inclusiveStartingColumnIndexOfCurrentToken = await
                CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                    cancellationToken);

            var currentColumnIndexWithIndexInPlainTextAccountedFor = inclusiveStartingColumnIndexOfCurrentToken +
                                                                     focusedPlainTextEditorRecord.CurrentTextToken
                                                                         .IndexInPlainText!.Value;

            var targetRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex + 1;

            var belowRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.Rows[targetRowIndex]);

            var tokenInRowBelowTuple = await CalculateTokenAtColumnIndexRespectiveToRowAsync(
                focusedPlainTextEditorRecord,
                belowRow
                    as PlainTextEditorRow
                ?? throw new ApplicationException($"Expected type {nameof(PlainTextEditorRow)}"),
                currentColumnIndexWithIndexInPlainTextAccountedFor,
                cancellationToken);

            var currentRow = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();
            var currentToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>();

            var currentRowReplacement = currentRow with
            {
                Tokens = currentRow.Tokens.Replace(currentToken, currentToken with
                {
                    IndexInPlainText = null
                }),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            int? indexInPlainText;

            if (currentColumnIndexWithIndexInPlainTextAccountedFor <
                tokenInRowBelowTuple.exclusiveEndingColumnIndex)
            {
                indexInPlainText = currentColumnIndexWithIndexInPlainTextAccountedFor -
                                   tokenInRowBelowTuple.inclusiveStartingColumnIndex;
            }
            else
            {
                indexInPlainText = tokenInRowBelowTuple.token.PlainText.Length - 1;
            }

            var belowRowReplacement = belowRow with
            {
                Tokens = belowRow.Tokens.Replace(tokenInRowBelowTuple.token, tokenInRowBelowTuple.token with
                {
                    IndexInPlainText = indexInPlainText
                }),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.Rows
                .Replace(currentRow, currentRowReplacement)
                .Replace(belowRow, belowRowReplacement);

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList,
                CurrentTokenIndex = tokenInRowBelowTuple.tokenIndex,
                CurrentRowIndex = targetRowIndex,
                CurrentCharacterColumnIndex = tokenInRowBelowTuple.inclusiveStartingColumnIndex 
                                             + indexInPlainText.Value,
                CurrentPositionIndex = focusedPlainTextEditorRecord
                                           .FileHandle.VirtualCharacterIndexMarkerForStartOfARow[targetRowIndex]
                                       + tokenInRowBelowTuple.inclusiveStartingColumnIndex
                                       + indexInPlainText.Value
            };
        }

        public static async Task<PlainTextEditorRecordBase> HandleArrowUpAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            if (focusedPlainTextEditorRecord.CurrentRowIndex <= 0)
                return focusedPlainTextEditorRecord;

            var inclusiveStartingColumnIndexOfCurrentToken =
                await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                    cancellationToken);

            var currentColumnIndexWithIndexInPlainTextAccountedFor = inclusiveStartingColumnIndexOfCurrentToken +
                                                                     focusedPlainTextEditorRecord.CurrentTextToken
                                                                         .IndexInPlainText!.Value;

            var targetRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex - 1;

            var aboveRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.Rows[targetRowIndex]);

            var tokenInRowAboveTuple = await CalculateTokenAtColumnIndexRespectiveToRowAsync(
                focusedPlainTextEditorRecord,
                aboveRow
                    as PlainTextEditorRow
                ?? throw new ApplicationException($"Expected type {nameof(PlainTextEditorRow)}"),
                currentColumnIndexWithIndexInPlainTextAccountedFor,
                cancellationToken);

            var currentRow = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();
            var currentToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>();

            var currentRowReplacement = currentRow with
            {
                Tokens = currentRow.Tokens.Replace(currentToken, currentToken with
                {
                    IndexInPlainText = null
                }),
                SequenceKey =SequenceKey.NewSequenceKey()
            };

            int? indexInPlainText;

            if (currentColumnIndexWithIndexInPlainTextAccountedFor <
                tokenInRowAboveTuple.exclusiveEndingColumnIndex)
            {
                indexInPlainText = currentColumnIndexWithIndexInPlainTextAccountedFor -
                                   tokenInRowAboveTuple.inclusiveStartingColumnIndex;
            }
            else
            {
                indexInPlainText = tokenInRowAboveTuple.token.PlainText.Length - 1;
            }

            var aboveRowReplacement = aboveRow with
            {
                Tokens = aboveRow.Tokens.Replace(tokenInRowAboveTuple.token, tokenInRowAboveTuple.token with
                {
                    IndexInPlainText = indexInPlainText
                }),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.Rows
                .Replace(currentRow, currentRowReplacement)
                .Replace(aboveRow, aboveRowReplacement);

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList,
                CurrentTokenIndex = tokenInRowAboveTuple.tokenIndex,
                CurrentRowIndex = targetRowIndex,
                CurrentCharacterColumnIndex = tokenInRowAboveTuple.inclusiveStartingColumnIndex
                                             + indexInPlainText.Value,
                CurrentPositionIndex = focusedPlainTextEditorRecord
                                           .FileHandle.VirtualCharacterIndexMarkerForStartOfARow[targetRowIndex]
                                       + tokenInRowAboveTuple.inclusiveStartingColumnIndex
                                       + indexInPlainText.Value
            };
        }

        public static async Task<PlainTextEditorRecordBase> HandleArrowRightAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            if (keyDownEventRecord.CtrlWasPressed)
            {
                var rememberTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;
                var rememberTokenWasWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                focusedPlainTextEditorRecord = await SetNextTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                    cancellationToken);

                var currentTokenIsWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                if ((rememberTokenWasWhitespace && currentTokenIsWhitespace) &&
                    (rememberTokenKey != focusedPlainTextEditorRecord.CurrentTextTokenKey))
                {
                    return await HandleMovementAsync(focusedPlainTextEditorRecord, 
                        keyDownEventRecord,
                        cancellationToken);
                }

                if (focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText !=
                    focusedPlainTextEditorRecord.CurrentTextToken.PlainText.Length - 1)
                {
                    var rememberIndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText!.Value;

                    var replacementToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>() with
                    {
                        IndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.PlainText.Length - 1
                    };

                    focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord,
                        replacementToken,
                        cancellationToken);

                    focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                    {
                        CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex
                            + (focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText!.Value - rememberIndexInPlainText),
                        CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex
                                               + (focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText!.Value - rememberIndexInPlainText)
                    };
                }

                return focusedPlainTextEditorRecord;
            }

            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            if (currentToken.IndexInPlainText == currentToken.PlainText.Length - 1)
            {
                return await SetNextTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                    cancellationToken);
            }
            else
            {
                var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = currentToken.IndexInPlainText + 1
                };

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex + 1,
                    CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex + 1
                };

                focusedPlainTextEditorRecord =
                    await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                        replacementCurrentToken,
                        cancellationToken);
            }

            return focusedPlainTextEditorRecord;
        }

        public static async Task<PlainTextEditorRecordBase> HandleHomeAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            int targetRowIndex = keyDownEventRecord.CtrlWasPressed
                ? 0
                : focusedPlainTextEditorRecord.CurrentRowIndex;

            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            var replacementCurrentToken = currentToken with
            {
                IndexInPlainText = null
            };

            focusedPlainTextEditorRecord =
                await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                    replacementCurrentToken,
                    cancellationToken);

            focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
            {
                CurrentTokenIndex = 0,
                CurrentRowIndex = targetRowIndex
            };

            currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            replacementCurrentToken = currentToken with
            {
                IndexInPlainText = 0
            };

            if (targetRowIndex == 0)
            {
                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentPositionIndex = 0
                };
            }
            else
            {
                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentPositionIndex = focusedPlainTextEditorRecord
                        .FileHandle.VirtualCharacterIndexMarkerForStartOfARow[focusedPlainTextEditorRecord.CurrentRowIndex]
                };
            }

            return await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);
        }

        public static async Task<PlainTextEditorRecordBase> HandleEndAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            int targetRowIndex = keyDownEventRecord.CtrlWasPressed
                ? focusedPlainTextEditorRecord.Rows.Count - 1
                : focusedPlainTextEditorRecord.CurrentRowIndex;

            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            var replacementCurrentToken = currentToken with
            {
                IndexInPlainText = null
            };

            focusedPlainTextEditorRecord =
                await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                    replacementCurrentToken,
                    cancellationToken);

            var row = focusedPlainTextEditorRecord.Rows[targetRowIndex];

            focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
            {
                CurrentTokenIndex = row.Tokens.Count - 1,
                CurrentRowIndex = targetRowIndex
            };

            currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            replacementCurrentToken = currentToken with
            {
                IndexInPlainText = currentToken.PlainText.Length - 1
            };

            return await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);
        }
    }
}