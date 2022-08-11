using System.Diagnostics;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static async Task<PlainTextEditorRecordBase> HandleKeyDownEventAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            // Useful for debugging:
            // Console.WriteLine($"HandleKeyDownEventAsync: {keyDownEventRecord.Key ?? keyDownEventRecord.Code ?? "null"}");
            
            if (KeyboardKeyFacts.IsWhitespaceKey(keyDownEventRecord))
            {
                return await HandleWhitespaceAsync(focusedPlainTextEditorRecord, 
                    keyDownEventRecord,
                    cancellationToken);
            }
            else if (KeyboardKeyFacts.IsMovementKey(keyDownEventRecord))
            {
                return await HandleMovementAsync(focusedPlainTextEditorRecord, 
                    keyDownEventRecord,
                    cancellationToken);
            }
            else if (KeyboardKeyFacts.IsMetaKey(keyDownEventRecord)) 
            {
                return await HandleMetaKeyAsync(focusedPlainTextEditorRecord, 
                    keyDownEventRecord,
                    cancellationToken);
            }
            else
            {
                return await HandleDefaultInsertAsync(focusedPlainTextEditorRecord, 
                    keyDownEventRecord,
                    cancellationToken);
            }
        }

        public static async Task<PlainTextEditorRecordBase> HandleOnClickEventAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord, 
            PlainTextEditorOnClickAction plainTextEditorOnClickAction,
            CancellationToken cancellationToken)
        {
            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = null
                };
            
            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);
    
            focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
            {
                CurrentTokenIndex = plainTextEditorOnClickAction.TokenIndex,
                CurrentRowIndex = plainTextEditorOnClickAction.RowIndex
            };

            currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = plainTextEditorOnClickAction.CharacterIndex ??
                        currentToken.PlainText.Length - 1
                };

            var startingCharacterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                false,
                cancellationToken);

            focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
            {
                CurrentCharacterColumnIndex = startingCharacterIndex
                    + replacementCurrentToken.GetIndexInPlainText(true),
                CurrentPositionIndex = focusedPlainTextEditorRecord
                                           .FileHandle.VirtualCharacterIndexMarkerForStartOfARow[focusedPlainTextEditorRecord.CurrentRowIndex]
                                       + startingCharacterIndex
                                       + replacementCurrentToken.GetIndexInPlainText(true)
            };

            return await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);
        }

        private static async Task<PlainTextEditorRecordBase> InsertNewCurrentTokenAfterCurrentPositionAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            ITextToken textToken,
            CancellationToken cancellationToken)
        {
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);

            var nextTokenList = focusedPlainTextEditorRecord.CurrentPlainTextEditorRow.Tokens
                .Insert(focusedPlainTextEditorRecord.CurrentTokenIndex + 1, textToken);

            var nextRowInstance = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>() with
            {
                Tokens = nextTokenList,
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.Rows.Replace(focusedPlainTextEditorRecord.CurrentPlainTextEditorRow,
                nextRowInstance);

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList,
                CurrentTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex + 1
            };
        }

        private static async Task<PlainTextEditorRecordBase> RemoveCurrentTokenAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.StartOfRow)
                return await RemoveStartOfRowTokenAsync(focusedPlainTextEditorRecord, cancellationToken);

            var toBeRemovedTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;
            var toBeChangedRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            focusedPlainTextEditorRecord = await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                cancellationToken);

            var toBeChangedRow = focusedPlainTextEditorRecord
                                         .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                                             focusedPlainTextEditorRecord.Rows[toBeChangedRowIndex]);

            var toBeRemovedToken = toBeChangedRow.Tokens[toBeRemovedTokenIndex];

            var nextTokenList = toBeChangedRow.Tokens
                .Remove(toBeRemovedToken);

            var nextRowList = focusedPlainTextEditorRecord.Rows
                .Replace(toBeChangedRow, toBeChangedRow with
                {
                    Tokens = nextTokenList,
                    SequenceKey = SequenceKey.NewSequenceKey()
                });

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList,
            };
        }

        private static async Task<PlainTextEditorRecordBase> RemoveStartOfRowTokenAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            if (focusedPlainTextEditorRecord.CurrentRowIndex == 0)
            {
                return focusedPlainTextEditorRecord;
            }
            
            if (focusedPlainTextEditorRecord.CurrentPlainTextEditorRow.Tokens.Count == 1)
            {
                return await RemoveCurrentRowAsync(focusedPlainTextEditorRecord, cancellationToken);
            }

            return focusedPlainTextEditorRecord;
        }

        private static async Task<PlainTextEditorRecordBase> RemoveCurrentRowAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            var rememberRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            focusedPlainTextEditorRecord = await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                cancellationToken);

            if (focusedPlainTextEditorRecord.CurrentRowIndex == rememberRowIndex - 1)
            {
                var nextRowList = focusedPlainTextEditorRecord.Rows.RemoveAt(focusedPlainTextEditorRecord.CurrentRowIndex + 1);

                return focusedPlainTextEditorRecord with
                {
                    Rows = nextRowList
                };
            }


            return focusedPlainTextEditorRecord;
        }

        private static async Task<PlainTextEditorRecordBase> ReplaceCurrentTokenWithAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            ITextToken textToken,
            CancellationToken cancellationToken)
        {
            var currentRow = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();
            var currentToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>();

            var nextRowInstance = currentRow with
            {
                Tokens = currentRow.Tokens.Replace(currentToken, textToken),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.Rows
                .Replace(focusedPlainTextEditorRecord.CurrentPlainTextEditorRow, nextRowInstance);

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList
            };
        }

        private static async Task<PlainTextEditorRecordBase> InsertNewLineAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            var rememberPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex;

            if (!keyDownEventRecord.IsForced &&
                focusedPlainTextEditorRecord is PlainTextEditorRecordMemoryMappedFile editorMemoryMappedFile)
            {
                var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(
                                         focusedPlainTextEditorRecord,
                                         true,
                                         cancellationToken)
                                     + focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true);

                await editorMemoryMappedFile.FileHandle.Edit
                    .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                        characterIndex,
                        focusedPlainTextEditorRecord.UseCarriageReturnNewLine
                            ? "\r\n"
                            : "\n",
                        cancellationToken);
            }

            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);

            var currentRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            var replacementRow = currentRow;
            var constructedRow = new PlainTextEditorRow(keyDownEventRecord);

            for (int i = focusedPlainTextEditorRecord.CurrentTokenIndex + 1; i < currentRow.Tokens.Count; i++)
            {
                var token = currentRow.Tokens[i];

                replacementRow = replacementRow with
                {
                    Tokens = replacementRow.Tokens.Remove(token),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                constructedRow = constructedRow with
                {
                    Tokens = constructedRow.Tokens.Add(token),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            var nextRowList = focusedPlainTextEditorRecord.Rows
                .Remove(currentRow)
                .InsertRange(focusedPlainTextEditorRecord.CurrentRowIndex,
                    new IPlainTextEditorRow[]
                    {
                        replacementRow,
                        constructedRow
                    });

            if (!keyDownEventRecord.IsForced)
            {
                focusedPlainTextEditorRecord.FileHandle.VirtualCharacterIndexMarkerForStartOfARow
                    .Insert(focusedPlainTextEditorRecord.CurrentRowIndex + 1,
                        rememberPositionIndex + 1);

                for (int i = focusedPlainTextEditorRecord.CurrentRowIndex + 2; i < focusedPlainTextEditorRecord.FileHandle.VirtualCharacterIndexMarkerForStartOfARow.Count; i++)
                {
                    focusedPlainTextEditorRecord.FileHandle.VirtualCharacterIndexMarkerForStartOfARow[i] += 1;
                }
            }

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList,
                CurrentTokenIndex = 0,
                CurrentRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex + 1,
                CurrentCharacterColumnIndex = 0,
                CurrentPositionIndex = focusedPlainTextEditorRecord
                    .FileHandle.VirtualCharacterIndexMarkerForStartOfARow[focusedPlainTextEditorRecord.CurrentRowIndex + 1]
            };
        }

        private static async Task<(int rowIndex, int tokenIndex, TextTokenBase token)> GetPreviousTokenTupleAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            if (focusedPlainTextEditorRecord.CurrentTokenIndex == 0)
            {
                if (focusedPlainTextEditorRecord.CurrentRowIndex > 0) 
                {
                    var rowIndex = focusedPlainTextEditorRecord.CurrentRowIndex - 1;

                    var row = focusedPlainTextEditorRecord.Rows[rowIndex];

                    var tokenIndex = row.Tokens.Count - 1;

                    var token = row.Tokens[tokenIndex];

                    return (
                        rowIndex, 
                        tokenIndex, 
                        token 
                            as TextTokenBase
                            ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                    );
                }

                return (
                    focusedPlainTextEditorRecord.CurrentRowIndex, 
                    focusedPlainTextEditorRecord.CurrentTokenIndex, 
                    focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>()
                );
            }
            else
            {
                var row = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

                var tokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex - 1;

                var token = row.Tokens[tokenIndex];

                return (
                    focusedPlainTextEditorRecord.CurrentRowIndex, 
                    tokenIndex, 
                    token 
                        as TextTokenBase
                        ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                );
            }
        }

        private static async Task<(int rowIndex, int tokenIndex, TextTokenBase token)> GetNextTokenTupleAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            var currentRow = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            if (focusedPlainTextEditorRecord.CurrentTokenIndex == currentRow.Tokens.Count - 1)
            {
                if (focusedPlainTextEditorRecord.CurrentRowIndex < focusedPlainTextEditorRecord.Rows.Count - 1) 
                {
                    var rowIndex = focusedPlainTextEditorRecord.CurrentRowIndex + 1;

                    var row = focusedPlainTextEditorRecord.Rows[rowIndex];

                    var tokenIndex = 0;

                    var token = row.Tokens[tokenIndex];

                    return (
                        rowIndex, 
                        tokenIndex, 
                        token 
                            as TextTokenBase
                            ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                    );
                }

                return (
                    focusedPlainTextEditorRecord.CurrentRowIndex, 
                    focusedPlainTextEditorRecord.CurrentTokenIndex, 
                    focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>()
                );
            }
            else
            {
                var tokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex + 1;

                var token = currentRow.Tokens[tokenIndex];

                return (
                    focusedPlainTextEditorRecord.CurrentRowIndex, 
                    tokenIndex, 
                    token 
                        as TextTokenBase
                        ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                );
            }
        }

        private static async Task<PlainTextEditorRecordBase> SetPreviousTokenAsCurrentAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            var rememberIndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true);

            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);

            var previousTokenTuple = await GetPreviousTokenTupleAsync(focusedPlainTextEditorRecord,
                cancellationToken);

            if (previousTokenTuple.rowIndex == focusedPlainTextEditorRecord.CurrentRowIndex)
            {
                if (previousTokenTuple.token.Key == focusedPlainTextEditorRecord.CurrentTextTokenKey)
                {
                    // No tokens previous to me
                    replacementCurrentToken = focusedPlainTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>() with
                        {
                            IndexInPlainText = 0
                        };

                    return await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                        replacementCurrentToken,
                        cancellationToken);
                }

                // There is a token previous to me on my current row
                var currentRow = focusedPlainTextEditorRecord
                    .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

                var replacementRow = currentRow with
                {
                    Tokens = currentRow.Tokens.Replace(previousTokenTuple.token, previousTokenTuple.token with
                    {
                        IndexInPlainText = previousTokenTuple.token.PlainText.Length - 1
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.Rows
                    .Replace(currentRow, replacementRow);

                return focusedPlainTextEditorRecord with
                {
                    Rows = nextRowList,
                    CurrentTokenIndex = previousTokenTuple.tokenIndex,
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex
                        - (rememberIndexInPlainText + 1),
                    CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex
                                           - (rememberIndexInPlainText + 1)
                };
            }
            else
            {
                // There was a previous token HOWEVER, it was located on previous row
                var previousRow = focusedPlainTextEditorRecord.Rows[previousTokenTuple.rowIndex]
                                         as PlainTextEditorRow
                                     ?? throw new ApplicationException($"Expected {nameof(PlainTextEditorRow)}");

                var replacementRow = previousRow with
                {
                    Tokens = previousRow.Tokens.Replace(previousTokenTuple.token, previousTokenTuple.token with
                    {
                        IndexInPlainText = previousTokenTuple.token.PlainText.Length - 1
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.Rows.Replace(previousRow, 
                    replacementRow);

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    Rows = nextRowList,
                    CurrentTokenIndex = previousTokenTuple.tokenIndex,
                    CurrentRowIndex = previousTokenTuple.rowIndex
                };

                var startingCharacterColumnIndex =
                    await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                        true,
                        cancellationToken);

                var actualCharacterColumnIndex = startingCharacterColumnIndex
                    + focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true);

                return focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = actualCharacterColumnIndex,
                    CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex
                                           - (rememberIndexInPlainText + 1)
                };
            }
        }

        private static async Task<PlainTextEditorRecordBase> SetNextTokenAsCurrentAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            var rememberToken = focusedPlainTextEditorRecord.CurrentTextToken;

            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                replacementCurrentToken,
                cancellationToken);

            var nextTokenTuple = await GetNextTokenTupleAsync(focusedPlainTextEditorRecord,
                cancellationToken);

            if (nextTokenTuple.rowIndex == focusedPlainTextEditorRecord.CurrentRowIndex)
            {
                if (nextTokenTuple.token.Key == focusedPlainTextEditorRecord.CurrentTextTokenKey)
                {
                    // No tokens next to me
                    replacementCurrentToken = focusedPlainTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>() with
                        {
                            IndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.PlainText.Length - 1
                        };

                    return await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord,
                        replacementCurrentToken,
                        cancellationToken);
                }

                // There is a token next to me on my current row
                var currentRow = focusedPlainTextEditorRecord
                    .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

                var replacementRow = currentRow with
                {
                    Tokens = currentRow.Tokens.Replace(nextTokenTuple.token, nextTokenTuple.token with
                    {
                        IndexInPlainText = 0
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.Rows
                    .Replace(currentRow, replacementRow);

                return focusedPlainTextEditorRecord with
                {
                    Rows = nextRowList,
                    CurrentTokenIndex = nextTokenTuple.tokenIndex,
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex
                        + (rememberToken.PlainText.Length - rememberToken.GetIndexInPlainText(true)),
                    CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex
                                           + (rememberToken.PlainText.Length - rememberToken.GetIndexInPlainText(true))
                };
            }
            else
            {
                // There was a next token HOWEVER, it was located on the next row
                var nextRow = focusedPlainTextEditorRecord.Rows[nextTokenTuple.rowIndex]
                    as PlainTextEditorRow
                    ?? throw new ApplicationException($"Expected {nameof(PlainTextEditorRow)}");

                var replacementRow = nextRow with
                {
                    Tokens = nextRow.Tokens.Replace(nextTokenTuple.token, nextTokenTuple.token with
                    {
                        IndexInPlainText = 0
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.Rows
                    .Replace(nextRow, replacementRow);

                return focusedPlainTextEditorRecord with
                {
                    Rows = nextRowList,
                    CurrentTokenIndex = nextTokenTuple.tokenIndex,
                    CurrentRowIndex = nextTokenTuple.rowIndex,
                    CurrentCharacterColumnIndex = 0,
                    CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex
                                           + (rememberToken.PlainText.Length - rememberToken.GetIndexInPlainText(true))
                };
            }
        }

        private static async Task<PlainTextEditorRecordBase> MoveCurrentRowToEndOfPreviousRowAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            var toBeMovedRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            var rememberTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;

            focusedPlainTextEditorRecord = await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                cancellationToken);

            if (focusedPlainTextEditorRecord.CurrentTextTokenKey == rememberTokenKey)
                return focusedPlainTextEditorRecord;

            var currentRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            var replacementRow = currentRow;

            for (int i = 1; i < toBeMovedRow.Tokens.Count; i++)
            {
                var token = toBeMovedRow.Tokens[i];

                replacementRow = replacementRow with
                {
                    Tokens = replacementRow.Tokens.Add(token),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            var nextRowList = focusedPlainTextEditorRecord.Rows.Replace(currentRow,
                    replacementRow)
                .RemoveAt(focusedPlainTextEditorRecord.CurrentRowIndex + 1);
            
            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList
            };
        }

        /// <summary>
		/// Returns the inclusive starting column index
        /// </summary>
		/// <param name="nextPlainTextEditorState"></param>
		/// <returns></returns>
		private static async Task<int> CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(
            PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            bool countTabsAsFourCharacters,
            CancellationToken cancellationToken)
		{
			var rollingCount = 0;
            var currentRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

			foreach (var token in currentRow.Tokens)
			{
				if (token.Key == focusedPlainTextEditorRecord.CurrentTextToken.Key)
                {
					return rollingCount;
				}
				else
				{
                    if (token is WhitespaceTextToken whitespace && whitespace.WhitespaceKind == WhitespaceKind.Tab)
                    {
                        if (countTabsAsFourCharacters)
                        {
                            // '    '
                            rollingCount += token.PlainText.Length;
                        }
                        else
                        {
                            // '\n'
                            rollingCount += token.CopyText.Length;
                        }
                    }
                    else
                    {
                        rollingCount += token.PlainText.Length;
                    }
				}
			}

			return 0;
		}

        private static async Task<(int inclusiveStartingColumnIndex, int exclusiveEndingColumnIndex, int tokenIndex, TextTokenBase token)> CalculateTokenAtColumnIndexRespectiveToRowAsync(
            PlainTextEditorRecordBase focusedPlainTextEditorRecord,
			PlainTextEditorRow row,
			int columnIndex,
            CancellationToken cancellationToken)
		{
			var rollingCount = 0;

            for (int i = 0; i < row.Tokens.Count; i++)
			{
                ITextToken token = row.Tokens[i];

				rollingCount += token.PlainText.Length;

				if (rollingCount > columnIndex || (i == row.Tokens.Count - 1))
				{
                    return (
                        rollingCount - token.PlainText.Length,
                        rollingCount,
                        i,
                        token as TextTokenBase
                            ?? throw new ApplicationException($"Expected type {nameof(TextTokenBase)}")
                    );
                }
			}

            throw new ApplicationException("Row was empty");
		}
    }
}
