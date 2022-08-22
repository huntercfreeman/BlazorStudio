using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static async Task<PlainTextEditorRecordBase> HandleDefaultInsertAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            var matchTextTokenKind = TextTokenKind.Default;
            
            if (KeyboardKeyFacts.IsPunctuationKey(keyDownEventRecord))
            {
                matchTextTokenKind = TextTokenKind.Punctuation;
            }
            
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == matchTextTokenKind)
            {
                // insert text to the current token

                var previousDefaultToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

                var content = previousDefaultToken.Content
                    .Insert(previousDefaultToken.GetIndexInPlainText(true) + 1, 
                        keyDownEventRecord.Key);

                if (!keyDownEventRecord.IsForced)
                {
                    var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                                             true,
                                             cancellationToken)
                                         + previousDefaultToken.GetIndexInPlainText(true);

                    await focusedPlainTextEditorRecord.FileHandle.Edit
                        .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                            characterIndex,
                            keyDownEventRecord.Key,
                            cancellationToken);
                }

                var nextDefaultToken = previousDefaultToken with
                {
                    Content = content,
                    IndexInPlainText = previousDefaultToken.GetIndexInPlainText(true) + 1
                };

                focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                {
                    CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex + 1,
                    CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex + 1
                };

                if (!keyDownEventRecord.IsForced)
                {
                    for (int i = focusedPlainTextEditorRecord.CurrentRowIndex + 1; i < focusedPlainTextEditorRecord.FileHandle.VirtualCharacterIndexMarkerForStartOfARow.Count; i++)
                    {
                        focusedPlainTextEditorRecord.FileHandle.VirtualCharacterIndexMarkerForStartOfARow[i] += 1;
                    }
                }

                return await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                    nextDefaultToken,
                    cancellationToken);
            }
            else
            {
                var nextTokenTuple = await GetNextTokenTupleAsync(focusedPlainTextEditorRecord,
                    cancellationToken);

                if (nextTokenTuple.rowIndex == focusedPlainTextEditorRecord.CurrentRowIndex &&
                    nextTokenTuple.token.Kind == matchTextTokenKind &&
                    focusedPlainTextEditorRecord.CurrentTextToken
                        .GetIndexInPlainText(true) == 
                            focusedPlainTextEditorRecord.CurrentTextToken.PlainText.Length - 1)
                {
                    // if at the end of the current token and the next
                    // token is of the same Kind prepend text to that next token

                    if (!keyDownEventRecord.IsForced)
                    {
                        var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                            true,
                            cancellationToken);

                        if (focusedPlainTextEditorRecord.CurrentTextToken is not WhitespaceTextToken whitespace ||
                            whitespace.WhitespaceKind != WhitespaceKind.Tab)
                        {
                            characterIndex += focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true);
                        }

                        await focusedPlainTextEditorRecord.FileHandle.Edit
                            .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                                characterIndex,
                                keyDownEventRecord.Key,
                                cancellationToken);
                    }

                    focusedPlainTextEditorRecord = await SetNextTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                        cancellationToken);
                    
                    var previousDefaultToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

                    var content = previousDefaultToken.Content
                        .Insert(0, keyDownEventRecord.Key);

                    var nextDefaultToken = previousDefaultToken with
                    {
                        Content = content,
                        IndexInPlainText = previousDefaultToken.GetIndexInPlainText(true)
                    };

                    focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                    {
                        CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex + 1,
                        CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex + 1
                    };

                    return await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                        nextDefaultToken,
                        cancellationToken);
                }
                else
                {
                    var rememberToken = focusedPlainTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>();

                    if (rememberToken.GetIndexInPlainText(true) != rememberToken.PlainText.Length - 1)
                    {
                        // if (active token is not a word, but the cursor is NOT at the end of that token then the token is split)

                        DefaultTextToken token;

                        if (KeyboardKeyFacts.IsPunctuationKey(keyDownEventRecord))
                        {
                            token = new PunctuationTextToken
                            {
                                Content = keyDownEventRecord.Key,
                                IndexInPlainText = 0
                            };
                        }
                        else
                        {
                            token = new DefaultTextToken
                            {
                                Content = keyDownEventRecord.Key,
                                IndexInPlainText = 0
                            };
                        }
                        
                        return await SplitCurrentTokenAsync(
                            focusedPlainTextEditorRecord, 
                            token,
                            keyDownEventRecord.IsForced,
                            cancellationToken
                        );
                    }
                    else
                    {
                        // if (active token is not a word, and the cursor is at the end of that token then insert a new token after the active one)

                        if (!keyDownEventRecord.IsForced)
                        {
                            var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                                                     true,
                                                     cancellationToken)
                                                 + focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true);

                            await focusedPlainTextEditorRecord.FileHandle.Edit
                                .InsertAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                                    characterIndex,
                                    keyDownEventRecord.Key,
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

                        DefaultTextToken token;

                        if (KeyboardKeyFacts.IsPunctuationKey(keyDownEventRecord))
                        {
                            token = new PunctuationTextToken
                            {
                                Content = keyDownEventRecord.Key,
                                IndexInPlainText = 0
                            };
                        }
                        else
                        {
                            token = new DefaultTextToken
                            {
                                Content = keyDownEventRecord.Key,
                                IndexInPlainText = 0
                            };
                        }

                        focusedPlainTextEditorRecord = focusedPlainTextEditorRecord with
                        {
                            CurrentCharacterColumnIndex = focusedPlainTextEditorRecord.CurrentCharacterColumnIndex + 1,
                            CurrentPositionIndex = focusedPlainTextEditorRecord.CurrentPositionIndex + 1
                        };

                        return await InsertNewCurrentTokenAfterCurrentPositionAsync(focusedPlainTextEditorRecord,
                            token,
                            cancellationToken);
                    }
                }
                
            }
        }

        public static async Task<PlainTextEditorRecordBase> HandleDefaultBackspaceAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord,
            CancellationToken cancellationToken)
        {
            var previousDefaultTextToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

            if (!keyDownEventRecord.IsForced)
            {
                var characterIndex = await CalculateCurrentTokenStartingCharacterIndexRespectiveToRowAsync(focusedPlainTextEditorRecord,
                                         true,
                                         cancellationToken)
                                     + focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true);

                await focusedPlainTextEditorRecord.FileHandle.Edit
                    .RemoveAsync(focusedPlainTextEditorRecord.CurrentRowIndex,
                        characterIndex - 1,
                        characterCount: 1 + previousDefaultTextToken.GetIndexInPlainText(true),
                        cancellationToken: cancellationToken);
            }
            
            string firstSplitContent;

            if (keyDownEventRecord.CtrlWasPressed)
            {
                firstSplitContent = string.Empty;
            }
            else
            {
                firstSplitContent =  previousDefaultTextToken.Content
                    .Substring(0, previousDefaultTextToken.GetIndexInPlainText(true));
            }
            
            var secondSplitContent = string.Empty;

            if (previousDefaultTextToken.GetIndexInPlainText(true) != previousDefaultTextToken.Content.Length - 1)
            {
                secondSplitContent = previousDefaultTextToken.Content
                    .Substring(previousDefaultTextToken.GetIndexInPlainText(true) + 1);
            }

            int? nextIndexInPlainText;

            if (keyDownEventRecord.CtrlWasPressed)
            {
                nextIndexInPlainText = previousDefaultTextToken.GetIndexInPlainText(true) -
                                       (1 + previousDefaultTextToken.GetIndexInPlainText(true));
            }
            else
            {
                nextIndexInPlainText = previousDefaultTextToken.GetIndexInPlainText(true) - 1;
            }
            
            var nextDefaultToken = previousDefaultTextToken with
                {
                    Content = firstSplitContent + secondSplitContent,
                    IndexInPlainText = nextIndexInPlainText
                };

            if (nextDefaultToken.Content.Length == 0)
                return await RemoveCurrentTokenAsync(focusedPlainTextEditorRecord,
                    cancellationToken);

            focusedPlainTextEditorRecord = await ReplaceCurrentTokenWithAsync(focusedPlainTextEditorRecord, 
                nextDefaultToken,
                cancellationToken);

            if (nextDefaultToken.GetIndexInPlainText(true) == -1)
                return await SetPreviousTokenAsCurrentAsync(focusedPlainTextEditorRecord,
                    cancellationToken);

            return focusedPlainTextEditorRecord;
        }
    }
}
