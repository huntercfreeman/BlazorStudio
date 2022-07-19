using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlainTextEditor.ClassLib.Keyboard;
using PlainTextEditor.ClassLib.Sequence;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

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
                return SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);
            }
            else
            {
                var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = currentToken.IndexInPlainText - 1
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
                focusedPlainTextEditorRecord.List.Count - 1)
            {
                return focusedPlainTextEditorRecord;
            }

            var inclusiveStartingColumnIndexOfCurrentToken =
                CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord);

            var currentColumnIndexWithIndexInPlainTextAccountedFor = inclusiveStartingColumnIndexOfCurrentToken +
                                                                     focusedPlainTextEditorRecord.CurrentTextToken
                                                                         .IndexInPlainText!.Value;

            var targetRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex + 1;

            var belowRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.List[targetRowIndex]);

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
                List = currentRow.List.Replace(currentToken, currentToken with
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
                List = belowRow.List.Replace(tokenInRowBelowTuple.token, tokenInRowBelowTuple.token with
                {
                    IndexInPlainText = indexInPlainText
                }),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.List
                .Replace(currentRow, currentRowReplacement)
                .Replace(belowRow, belowRowReplacement);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = tokenInRowBelowTuple.tokenIndex,
                CurrentRowIndex = targetRowIndex,
            };
        }

        public static PlainTextEditorRecord HandleArrowUp(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentRowIndex <= 0)
                return focusedPlainTextEditorRecord;

            var inclusiveStartingColumnIndexOfCurrentToken =
                CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord);

            var currentColumnIndexWithIndexInPlainTextAccountedFor = inclusiveStartingColumnIndexOfCurrentToken +
                                                                     focusedPlainTextEditorRecord.CurrentTextToken
                                                                         .IndexInPlainText!.Value;

            var targetRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex - 1;

            var aboveRow = focusedPlainTextEditorRecord
                .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                    focusedPlainTextEditorRecord.List[targetRowIndex]);

            var tokenInRowAboveMetaData = CalculateTokenAtColumnIndexRespectiveToRow(
                focusedPlainTextEditorRecord,
                aboveRow
                    as PlainTextEditorRow
                ?? throw new ApplicationException($"Expected type {nameof(PlainTextEditorRow)}"),
                currentColumnIndexWithIndexInPlainTextAccountedFor);

            var currentRow = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();
            var currentToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>();

            var currentRowReplacement = currentRow with
            {
                List = currentRow.List.Replace(currentToken, currentToken with
                {
                    IndexInPlainText = null
                }),
                SequenceKey =SequenceKey.NewSequenceKey()
            };

            int? indexInPlainText;

            if (currentColumnIndexWithIndexInPlainTextAccountedFor <
                tokenInRowAboveMetaData.exclusiveEndingColumnIndex)
            {
                indexInPlainText = currentColumnIndexWithIndexInPlainTextAccountedFor -
                                   tokenInRowAboveMetaData.inclusiveStartingColumnIndex;
            }
            else
            {
                indexInPlainText = tokenInRowAboveMetaData.token.PlainText.Length - 1;
            }

            var aboveRowReplacement = aboveRow with
            {
                List = aboveRow.List.Replace(tokenInRowAboveMetaData.token, tokenInRowAboveMetaData.token with
                {
                    IndexInPlainText = indexInPlainText
                }),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.List
                .Replace(currentRow, currentRowReplacement)
                .Replace(aboveRow, aboveRowReplacement);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = tokenInRowAboveMetaData.tokenIndex,
                CurrentRowIndex = targetRowIndex
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
                return SetNextTokenAsCurrent(focusedPlainTextEditorRecord);
            }
            else
            {
                var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = currentToken.IndexInPlainText + 1
                };

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
                ? focusedPlainTextEditorRecord.List.Count - 1
                : focusedPlainTextEditorRecord.CurrentRowIndex;

            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            var replacementCurrentToken = currentToken with
            {
                IndexInPlainText = null
            };

            focusedPlainTextEditorRecord =
                ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var row = focusedPlainTextEditorRecord.List[targetRowIndex];

            focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
            {
                CurrentTokenIndex = row.List.Count - 1,
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