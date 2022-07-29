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
                var previousDefaultToken = focusedPlainTextEditorRecord.GetCurrentTextTokenAs<DefaultTextToken>();

                var content = previousDefaultToken.Content
                    .Insert(previousDefaultToken.IndexInPlainText!.Value + 1, keyDownEventRecord.Key);

                var characterIndex = CalculateCurrentTokenColumnIndexRespectiveToRow(focusedPlainTextEditorRecord)
                    + previousDefaultToken.IndexInPlainText.Value;

                focusedPlainTextEditorRecord.FileHandle.Edit
                    .Insert(focusedPlainTextEditorRecord.CurrentRowIndex, 
                        characterIndex,
                        keyDownEventRecord.Key);

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
                        return SplitCurrentToken(
                            focusedPlainTextEditorRecord, 
                            new DefaultTextToken
                            {
                                Content = keyDownEventRecord.Key,
                                IndexInPlainText = 0
                            }
                        );
                    }
                    else
                    {
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
