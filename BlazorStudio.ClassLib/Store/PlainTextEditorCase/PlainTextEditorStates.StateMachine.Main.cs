using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static PlainTextEditorRecord HandleKeyDownEvent(PlainTextEditorRecord focusedPlainTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            if (KeyboardKeyFacts.IsWhitespaceKey(keyDownEventRecord))
            {
                return HandleWhitespace(focusedPlainTextEditorRecord, keyDownEventRecord);
            }
            else if (KeyboardKeyFacts.IsMovementKey(keyDownEventRecord))
            {
                return HandleMovement(focusedPlainTextEditorRecord, keyDownEventRecord);
            }
            else if (KeyboardKeyFacts.IsMetaKey(keyDownEventRecord)) 
            {
                return HandleMetaKey(focusedPlainTextEditorRecord, keyDownEventRecord);
            }
            else
            {
                return HandleDefaultInsert(focusedPlainTextEditorRecord, keyDownEventRecord);
            }
        }
        
        public static PlainTextEditorRecord HandleOnClickEvent(PlainTextEditorRecord focusedPlainTextEditorRecord, 
            PlainTextEditorOnClickAction plainTextEditorOnClickAction)
        {
            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();

            var replacementCurrentToken = currentToken with
                {
                    IndexInPlainText = null
                };
            
            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
    
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

            return ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
        }

        private static PlainTextEditorRecord InsertNewCurrentTokenAfterCurrentPosition(PlainTextEditorRecord focusedPlainTextEditorRecord,
            ITextToken textToken)
        {
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var nextTokenList = focusedPlainTextEditorRecord.CurrentPlainTextEditorRow.List
                .Insert(focusedPlainTextEditorRecord.CurrentTokenIndex + 1, textToken);

            var nextRowInstance = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>() with
            {
                List = nextTokenList,
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.List.Replace(focusedPlainTextEditorRecord.CurrentPlainTextEditorRow,
                nextRowInstance);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex + 1
            };
        }
        
        private static PlainTextEditorRecord RemoveCurrentToken(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.StartOfRow)
                return RemoveStartOfRowToken(focusedPlainTextEditorRecord);

            var toBeRemovedTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;
            var toBeChangedRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);

            var toBeChangedRow = focusedPlainTextEditorRecord
                                         .ConvertIPlainTextEditorRowAs<PlainTextEditorRow>(
                                             focusedPlainTextEditorRecord.List[toBeChangedRowIndex]);

            var toBeRemovedToken = toBeChangedRow.List[toBeRemovedTokenIndex];

            var nextTokenList = toBeChangedRow.List
                .Remove(toBeRemovedToken);

            var nextRowList = focusedPlainTextEditorRecord.List
                .Replace(toBeChangedRow, toBeChangedRow with
                {
                    List = nextTokenList,
                    SequenceKey = SequenceKey.NewSequenceKey()
                });

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
            };
        }
        
        private static PlainTextEditorRecord RemoveStartOfRowToken(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentRowIndex == 0)
            {
                return focusedPlainTextEditorRecord;
            }
            
            if (focusedPlainTextEditorRecord.CurrentPlainTextEditorRow.List.Count == 1)
            {
                return RemoveCurrentRow(focusedPlainTextEditorRecord);
            }

            return focusedPlainTextEditorRecord;
        }

        private static PlainTextEditorRecord RemoveCurrentRow(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            var rememberRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);

            if (focusedPlainTextEditorRecord.CurrentRowIndex == rememberRowIndex - 1)
            {
                var nextRowList = focusedPlainTextEditorRecord.List.RemoveAt(focusedPlainTextEditorRecord.CurrentRowIndex + 1);

                return focusedPlainTextEditorRecord with
                {
                    List = nextRowList
                };
            }


            return focusedPlainTextEditorRecord;
        }
        
        private static PlainTextEditorRecord ReplaceCurrentTokenWith(PlainTextEditorRecord focusedPlainTextEditorRecord,
            ITextToken textToken)
        {
            var currentRow = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();
            var currentToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<TextTokenBase>();

            var nextRowInstance = currentRow with
            {
                List = currentRow.List.Replace(currentToken, textToken),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.List
                .Replace(focusedPlainTextEditorRecord.CurrentPlainTextEditorRow, nextRowInstance);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList
            };
        }

        private static PlainTextEditorRecord InsertNewLine(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var currentRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            var replacementRow = currentRow;
            var constructedRow = new PlainTextEditorRow(keyDownEventRecord);

            for (int i = focusedPlainTextEditorRecord.CurrentTokenIndex + 1; i < currentRow.List.Count; i++)
            {
                var token = currentRow.List[i];

                replacementRow = replacementRow with
                {
                    List = replacementRow.List.Remove(token),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                constructedRow = constructedRow with
                {
                    List = constructedRow.List.Add(token),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            var nextRowList = focusedPlainTextEditorRecord.List
                .Remove(currentRow)
                .InsertRange(focusedPlainTextEditorRecord.CurrentRowIndex,
                    new IPlainTextEditorRow[]
                    {
                        replacementRow,
                        constructedRow
                    });

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = 0,
                CurrentRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex + 1
            };
        }
        
        private static (int rowIndex, int tokenIndex, TextTokenBase token) GetPreviousTokenTuple(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentTokenIndex == 0)
            {
                if (focusedPlainTextEditorRecord.CurrentRowIndex > 0) 
                {
                    var rowIndex = focusedPlainTextEditorRecord.CurrentRowIndex - 1;

                    var row = focusedPlainTextEditorRecord.List[rowIndex];

                    var tokenIndex = row.List.Count - 1;

                    var token = row.List[tokenIndex];

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

                var token = row.List[tokenIndex];

                return (
                    focusedPlainTextEditorRecord.CurrentRowIndex, 
                    tokenIndex, 
                    token 
                        as TextTokenBase
                        ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                );
            }
        }
        
        private static (int rowIndex, int tokenIndex, TextTokenBase token) GetNextTokenTuple(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            var currentRow = focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            if (focusedPlainTextEditorRecord.CurrentTokenIndex == currentRow.List.Count - 1)
            {
                if (focusedPlainTextEditorRecord.CurrentRowIndex < focusedPlainTextEditorRecord.List.Count - 1) 
                {
                    var rowIndex = focusedPlainTextEditorRecord.CurrentRowIndex + 1;

                    var row = focusedPlainTextEditorRecord.List[rowIndex];

                    var tokenIndex = 0;

                    var token = row.List[tokenIndex];

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

                var token = currentRow.List[tokenIndex];

                return (
                    focusedPlainTextEditorRecord.CurrentRowIndex, 
                    tokenIndex, 
                    token 
                        as TextTokenBase
                        ?? throw new ApplicationException($"Expected {nameof(TextTokenBase)}")
                );
            }
        }
        
        private static PlainTextEditorRecord SetPreviousTokenAsCurrent(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var previousTokenTuple = GetPreviousTokenTuple(focusedPlainTextEditorRecord);

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

                    return ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
                }

                // There is a token previous to me on my current row
                var currentRow = focusedPlainTextEditorRecord
                    .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

                var replacementRow = currentRow with
                {
                    List = currentRow.List.Replace(previousTokenTuple.token, previousTokenTuple.token with
                    {
                        IndexInPlainText = previousTokenTuple.token.PlainText.Length - 1
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.List
                    .Replace(currentRow, replacementRow);

                return focusedPlainTextEditorRecord with
                {
                    List = nextRowList,
                    CurrentTokenIndex = previousTokenTuple.tokenIndex
                };
            }
            else
            {
                // There was a previous token HOWEVER, it was located on previous row
                var previousRow = focusedPlainTextEditorRecord.List[previousTokenTuple.rowIndex]
                                         as PlainTextEditorRow
                                     ?? throw new ApplicationException($"Expected {nameof(PlainTextEditorRow)}");

                var replacementRow = previousRow with
                {
                    List = previousRow.List.Replace(previousTokenTuple.token, previousTokenTuple.token with
                    {
                        IndexInPlainText = previousTokenTuple.token.PlainText.Length - 1
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.List.Replace(previousRow, 
                    replacementRow);

                return focusedPlainTextEditorRecord with
                {
                    List = nextRowList,
                    CurrentTokenIndex = previousTokenTuple.tokenIndex,
                    CurrentRowIndex = previousTokenTuple.rowIndex
                };
            }
        }
        
        private static PlainTextEditorRecord SetNextTokenAsCurrent(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var nextTokenTuple = GetNextTokenTuple(focusedPlainTextEditorRecord);

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

                    return ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
                }

                // There is a token next to me on my current row
                var currentRow = focusedPlainTextEditorRecord
                    .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

                var replacementRow = currentRow with
                {
                    List = currentRow.List.Replace(nextTokenTuple.token, nextTokenTuple.token with
                    {
                        IndexInPlainText = 0
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.List
                    .Replace(currentRow, replacementRow);

                return focusedPlainTextEditorRecord with
                {
                    List = nextRowList,
                    CurrentTokenIndex = nextTokenTuple.tokenIndex
                };
            }
            else
            {
                // There was a next token HOWEVER, it was located on the next row
                var nextRow = focusedPlainTextEditorRecord.List[nextTokenTuple.rowIndex]
                    as PlainTextEditorRow
                    ?? throw new ApplicationException($"Expected {nameof(PlainTextEditorRow)}");

                var replacementRow = nextRow with
                {
                    List = nextRow.List.Replace(nextTokenTuple.token, nextTokenTuple.token with
                    {
                        IndexInPlainText = 0
                    }),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                var nextRowList = focusedPlainTextEditorRecord.List
                    .Replace(nextRow, replacementRow);

                return focusedPlainTextEditorRecord with
                {
                    List = nextRowList,
                    CurrentTokenIndex = nextTokenTuple.tokenIndex,
                    CurrentRowIndex = nextTokenTuple.rowIndex
                };
            }
        }

        private static PlainTextEditorRecord MoveCurrentRowToEndOfPreviousRow(PlainTextEditorRecord focusedPlainTextEditorRecord)
        {
            var toBeMovedRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            var rememberTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;

            focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);

            if (focusedPlainTextEditorRecord.CurrentTextTokenKey == rememberTokenKey)
                return focusedPlainTextEditorRecord;

            var currentRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            var replacementRow = currentRow;

            for (int i = 1; i < toBeMovedRow.List.Count; i++)
            {
                var token = toBeMovedRow.List[i];

                replacementRow = replacementRow with
                {
                    List = replacementRow.List.Add(token),
                    SequenceKey = SequenceKey.NewSequenceKey()
                };
            }

            var nextRowList = focusedPlainTextEditorRecord.List.Replace(currentRow,
                    replacementRow)
                .RemoveAt(focusedPlainTextEditorRecord.CurrentRowIndex + 1);
            
            return focusedPlainTextEditorRecord with
            {
                List = nextRowList
            };
        }

        /// <summary>
		/// Returns the inclusive starting column index
		/// </summary>
		/// <param name="nextPlainTextEditorState"></param>
		/// <returns></returns>
		private static int CalculateCurrentTokenColumnIndexRespectiveToRow(
			PlainTextEditorRecord focusedPlainTextEditorRecord)
		{
			var rollingCount = 0;
            var currentRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

			foreach (var token in currentRow.List)
			{
				if (token.Key == focusedPlainTextEditorRecord.CurrentTextToken.Key)
                {
					return rollingCount;
				}
				else
				{
					rollingCount += token.PlainText.Length;
				}
			}

			return 0;
		}

        private static (int inclusiveStartingColumnIndex, int exclusiveEndingColumnIndex, int tokenIndex, TextTokenBase token) CalculateTokenAtColumnIndexRespectiveToRow(
			PlainTextEditorRecord focusedPlainTextEditorRecord,
			PlainTextEditorRow row,
			int columnIndex)
		{
			var rollingCount = 0;

            for (int i = 0; i < row.List.Count; i++)
			{
                ITextToken token = row.List[i];

				rollingCount += token.PlainText.Length;

				if (rollingCount > columnIndex || (i == row.List.Count - 1))
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
