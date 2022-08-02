using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static PlainTextEditorRecord HandleMovement(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT_KEY:
                    return HandleArrowLeft(focusedPlainTextEditorRecord, keyDownEventRecord);
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN_KEY:
                    return HandleArrowDown(focusedPlainTextEditorRecord, keyDownEventRecord);
                case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP_KEY:
                    return HandleArrowUp(focusedPlainTextEditorRecord, keyDownEventRecord);
                case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT_KEY:
                    return HandleArrowRight(focusedPlainTextEditorRecord, keyDownEventRecord);
                case KeyboardKeyFacts.MovementKeys.HOME_KEY:
                    return HandleHome(focusedPlainTextEditorRecord, keyDownEventRecord);
                case KeyboardKeyFacts.MovementKeys.END_KEY:
                    return HandleEnd(focusedPlainTextEditorRecord, keyDownEventRecord);
            }

            return focusedPlainTextEditorRecord;
        }

        public static PlainTextEditorRecord HandleArrowLeft(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            if (keyDownEventRecord.CtrlWasPressed)
            {
                var rememberTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;
                var rememberTokenWasWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex
                        - focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText!.Value
                };

                focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);

                var currentTokenIsWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                if ((rememberTokenWasWhitespace && currentTokenIsWhitespace) &&
                    (rememberTokenKey != focusedPlainTextEditorRecord.CurrentTextTokenKey))
                {
                    return HandleMovement(focusedPlainTextEditorRecord, keyDownEventRecord);
                }

                return focusedPlainTextEditorRecord;
            }

            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            if (currentToken.IndexInPlainText == 0)
            {
                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex - 1
                };

                return SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);
            }
            else
            {
                var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = currentToken.IndexInPlainText - 1
                };

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex - 1
                };

                focusedPlainTextEditorRecord =
                    ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
            }

            return focusedPlainTextEditorRecord;
        }

        public static PlainTextEditorRecord HandleArrowDown(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentRowIndex >=
                focusedPlainTextEditorRecord.Rows.Count - 1)
            {
                return focusedPlainTextEditorRecord;
            }

            var inclusiveStartingColumnIndexOfCurrentToken =
                CalculateCurrentTokenStartingCharacterIndexRespectiveToRow(focusedPlainTextEditorRecord);

            var currentColumnIndexWithIndexInPlainTextAccountedFor = inclusiveStartingColumnIndexOfCurrentToken +
                                                                     focusedPlainTextEditorRecord.CurrentTextToken
                                                                         .IndexInPlainText!.Value;

            var targetRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex + 1;

            var belowRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.Rows[targetRowIndex]);

            var tokenInRowBelowTuple = CalculateTokenAtColumnIndexRespectiveToRow(
                focusedPlainTextEditorRecord,
                belowRow
                    as PlainTextEditorRow
                ?? throw new ApplicationException($"Expected type {nameof(PlainTextEditorRow)}"),
                currentColumnIndexWithIndexInPlainTextAccountedFor);

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
                CharacterColumnIndexOffset = tokenInRowBelowTuple.inclusiveStartingColumnIndex 
                                             + indexInPlainText.Value
            };
        }

        public static PlainTextEditorRecord HandleArrowUp(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentRowIndex <= 0)
                return focusedPlainTextEditorRecord;

            var inclusiveStartingColumnIndexOfCurrentToken =
                CalculateCurrentTokenStartingCharacterIndexRespectiveToRow(focusedPlainTextEditorRecord);

            var currentColumnIndexWithIndexInPlainTextAccountedFor = inclusiveStartingColumnIndexOfCurrentToken +
                                                                     focusedPlainTextEditorRecord.CurrentTextToken
                                                                         .IndexInPlainText!.Value;

            var targetRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex - 1;

            var aboveRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.Rows[targetRowIndex]);

            var tokenInRowAboveTuple = CalculateTokenAtColumnIndexRespectiveToRow(
                focusedPlainTextEditorRecord,
                aboveRow
                    as PlainTextEditorRow
                ?? throw new ApplicationException($"Expected type {nameof(PlainTextEditorRow)}"),
                currentColumnIndexWithIndexInPlainTextAccountedFor);

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
                CharacterColumnIndexOffset = tokenInRowAboveTuple.inclusiveStartingColumnIndex
                                             + indexInPlainText.Value
            };
        }

        public static PlainTextEditorRecord HandleArrowRight(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            if (keyDownEventRecord.CtrlWasPressed)
            {
                var rememberTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;
                var rememberTokenWasWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex 
                                                  + (focusedPlainTextEditorRecord.CurrentTextToken.PlainText.Length
                                                     - focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText!.Value)
                };

                focusedPlainTextEditorRecord = SetNextTokenAsCurrent(focusedPlainTextEditorRecord);

                var currentTokenIsWhitespace =
                    focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Whitespace;

                if ((rememberTokenWasWhitespace && currentTokenIsWhitespace) &&
                    (rememberTokenKey != focusedPlainTextEditorRecord.CurrentTextTokenKey))
                {
                    return HandleMovement(focusedPlainTextEditorRecord, keyDownEventRecord);
                }

                if (focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText !=
                    focusedPlainTextEditorRecord.CurrentTextToken.PlainText.Length - 1)
                {
                    var replacementToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>() with
                    {
                        IndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.PlainText.Length - 1
                    };

                    focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord,
                        replacementToken);
                }

                return focusedPlainTextEditorRecord;
            }

            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            if (currentToken.IndexInPlainText == currentToken.PlainText.Length - 1)
            {
                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex + 1
                };

                return SetNextTokenAsCurrent(focusedPlainTextEditorRecord);
            }
            else
            {
                var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = currentToken.IndexInPlainText + 1
                };

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex + 1
                };

                //if (focusedPlainTextEditorRecord.SelectionSpan is null)
                //{
                //    focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                //    {
                //        SelectionSpan = new()
                //        {
                //            InclusiveStartingDocumentTextIndex = 0,
                //            ExclusiveEndingDocumentTextIndex = 0
                //        }
                //    };
                //}

                focusedPlainTextEditorRecord =
                    ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
            }

            return focusedPlainTextEditorRecord;
        }

        public static PlainTextEditorRecord HandleHome(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
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
                ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
            {
                CurrentTokenIndex = 0,
                CurrentRowIndex = targetRowIndex
            };

            currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            replacementCurrentToken = currentToken with
            {
                IndexInPlainText = currentToken.PlainText.Length - 1
            };

            return ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
        }

        public static PlainTextEditorRecord HandleEnd(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
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
                ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

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

            return ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
        }
    }
}