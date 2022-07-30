using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static PlainTextEditorRecord HandleDefaultInsert(PlainTextEditorRecord focusedPlainTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Default)
            {
                // if (active token is a word)

                var previousDefaultToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

                var content = previousDefaultToken.Content
                    .Insert(previousDefaultToken.IndexInPlainText!.Value + 1, keyDownEventRecord.Key);

                if (!keyDownEventRecord.IsForced)
                {
                    var characterIndex = CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord)
                                         + previousDefaultToken.IndexInPlainText.Value;

                    focusedPlainTextEditorRecord.FileHandle.Edit
                        .Insert(focusedPlainTextEditorRecord.CurrentRowIndex,
                            characterIndex,
                            keyDownEventRecord.Key);
                }

                var nextDefaultToken = previousDefaultToken with
                {
                    Content = content,
                    IndexInPlainText = previousDefaultToken.IndexInPlainText + 1
                };
                
                return ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, nextDefaultToken);
            }
            else
            {
                var nextTokenTuple = GetNextTokenTuple(focusedPlainTextEditorRecord);

                if (nextTokenTuple.rowIndex == focusedPlainTextEditorRecord.CurrentRowIndex &&
                    nextTokenTuple.token.Kind == TextTokenKind.Default)
                {
                    // if (active token is not a word, and the next token is a word however then prepend text to that next token)

                    if (!keyDownEventRecord.IsForced)
                    {
                        var characterIndex = CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord);

                        if (focusedPlainTextEditorRecord.CurrentTextToken is not WhitespaceTextToken whitespace ||
                            whitespace.WhitespaceKind != WhitespaceKind.Tab)
                        {
                            characterIndex += focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;
                        }

                        focusedPlainTextEditorRecord.FileHandle.Edit
                            .Insert(focusedPlainTextEditorRecord.CurrentRowIndex,
                                characterIndex,
                                keyDownEventRecord.Key);
                    }

                    focusedPlainTextEditorRecord = SetNextTokenAsCurrent(focusedPlainTextEditorRecord);
                    
                    var previousDefaultToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

                    var content = previousDefaultToken.Content
                        .Insert(0, keyDownEventRecord.Key);

                    var nextDefaultToken = previousDefaultToken with
                    {
                        Content = content,
                        IndexInPlainText = previousDefaultToken.IndexInPlainText
                    };
                    
                    return ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, nextDefaultToken);
                }
                else
                {
                    var rememberToken = focusedPlainTextEditorRecord
                        .GetCurrentTextTokenAs<TextTokenBase>();

                    if (rememberToken.IndexInPlainText!.Value != rememberToken.PlainText.Length - 1)
                    {
                        // if (active token is not a word, but the cursor is NOT at the end of that token the token is split)
                        
                        return SplitCurrentToken(
                            focusedPlainTextEditorRecord, 
                            new DefaultTextToken
                            {
                                Content = keyDownEventRecord.Key,
                                IndexInPlainText = 0
                            },
                            keyDownEventRecord.IsForced
                        );
                    }
                    else
                    {
                        // if (active token is not a word, and the cursor is at the end of that token then insert a new 'word token' after the active one)

                        if (!keyDownEventRecord.IsForced)
                        {
                            var characterIndex = CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord)
                                                 + focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText.Value;

                            focusedPlainTextEditorRecord.FileHandle.Edit
                                .Insert(focusedPlainTextEditorRecord.CurrentRowIndex,
                                    characterIndex,
                                    keyDownEventRecord.Key);
                        }

                        var replacementCurrentToken = focusedPlainTextEditorRecord
                            .GetCurrentTextTokenAs<TextTokenBase>() with
                            {
                                IndexInPlainText = null
                            };

                        focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

                        var defaultTextToken = new DefaultTextToken
                        {
                            Content = keyDownEventRecord.Key,
                            IndexInPlainText = 0
                        };

                        return InsertNewCurrentTokenAfterCurrentPosition(focusedPlainTextEditorRecord,
                            defaultTextToken);
                    }
                }
                
            }
        }
        
        public static PlainTextEditorRecord HandleDefaultBackspace(PlainTextEditorRecord focusedPlainTextEditorRecord, 
            KeyDownEventRecord keyDownEventRecord)
        {
            var previousDefaultTextToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

            var firstSplitContent = previousDefaultTextToken.Content
                .Substring(0, previousDefaultTextToken.IndexInPlainText!.Value);

            var secondSplitContent = string.Empty;

            if (previousDefaultTextToken.IndexInPlainText != previousDefaultTextToken.Content.Length - 1)
            {
                secondSplitContent = previousDefaultTextToken.Content
                    .Substring(previousDefaultTextToken.IndexInPlainText!.Value + 1);
            }

            var nextDefaultToken = previousDefaultTextToken with
                {
                    Content = firstSplitContent + secondSplitContent,
                    IndexInPlainText = previousDefaultTextToken.IndexInPlainText - 1
                };

            if (nextDefaultToken.Content.Length == 0)
                return RemoveCurrentToken(focusedPlainTextEditorRecord);

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, nextDefaultToken);

            if (nextDefaultToken.IndexInPlainText == -1)
                return SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);

            return focusedPlainTextEditorRecord;
        }
    }
}
