using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlainTextEditor.ClassLib.Keyboard;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        // Used when cursor is within text and the 'Enter' key is pressed as an example. That token would get split into two separate tokens.
        public static PlainTextEditorRecord SplitCurrentToken(PlainTextEditorRecord focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit)
        {
            var currentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>();
            
            switch (currentToken.Kind)
            {
                case TextTokenKind.Default:
                    return SplitDefaultToken(focusedPlainTextEditorRecord, tokenToInsertBetweenSplit);
                case TextTokenKind.Whitespace:
                    return SplitWhitespaceToken(focusedPlainTextEditorRecord, tokenToInsertBetweenSplit);
                default:
                    return focusedPlainTextEditorRecord;
            }
        }
        
        public static PlainTextEditorRecord SplitDefaultToken(PlainTextEditorRecord focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit)
        {            
            var rememberCurrentToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<DefaultTextToken>();

            var rememberTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;

            var firstSplitContent = rememberCurrentToken.Content
                .Substring(0, rememberCurrentToken.IndexInPlainText!.Value + 1);

            var secondSplitContent = rememberCurrentToken.Content
                    .Substring(rememberCurrentToken.IndexInPlainText!.Value + 1);

            var tokenFirst = new DefaultTextToken()
            {
                Content = firstSplitContent,
            };
            
            var tokenSecond = new DefaultTextToken()
            {
                Content = secondSplitContent
            };

            var toBeRemovedTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;
            var toBeChangedRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);
            
            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);

            var toBeChangedRow = focusedPlainTextEditorRecord.List[toBeChangedRowIndex];

            var nextRowBuilder = toBeChangedRow
                .With();

            nextRowBuilder.Remove(toBeRemovedTokenKey);

            int insertionOffset = 0;

            nextRowBuilder.Insert(rememberTokenIndex + insertionOffset++, tokenFirst);

            if (tokenToInsertBetweenSplit is not null)
            {
                nextRowBuilder.Insert(rememberTokenIndex + insertionOffset++, tokenToInsertBetweenSplit);
            }
            
            nextRowBuilder.Insert(rememberTokenIndex + insertionOffset++, tokenSecond);
            
            var nextRowInstance = nextRowBuilder.Build();
            
            var nextRowList = focusedPlainTextEditorRecord.List.Replace(toBeChangedRow,
                nextRowInstance);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex +
                    (tokenToInsertBetweenSplit is not null ? 2 : 1)
            };
        }

        public static PlainTextEditorRecord SplitWhitespaceToken(PlainTextEditorRecord focusedPlainTextEditorRecord,
            TextTokenBase? tokenToInsertBetweenSplit)
        {
            var rememberCurrentToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<WhitespaceTextToken>();

            if (rememberCurrentToken.WhitespaceKind != WhitespaceKind.Tab)
                return focusedPlainTextEditorRecord;

            var toBeRemovedTokenKey = focusedPlainTextEditorRecord.CurrentTextTokenKey;
            var toBeRemovedTokenIndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.IndexInPlainText;
            var rememberTokenIndex = focusedPlainTextEditorRecord.CurrentTokenIndex;
            var toBeChangedRowIndex = focusedPlainTextEditorRecord.CurrentRowIndex;

            focusedPlainTextEditorRecord = SetPreviousTokenAsCurrent(focusedPlainTextEditorRecord);


            var replacementCurrentToken = focusedPlainTextEditorRecord
                .GetCurrentTextTokenAs<TextTokenBase>() with
                {
                    IndexInPlainText = null
                };

            focusedPlainTextEditorRecord = ReplaceCurrentTokenWith(focusedPlainTextEditorRecord, replacementCurrentToken);
            
            var toBeChangedRow = focusedPlainTextEditorRecord.List[toBeChangedRowIndex];

            var nextRowBuilder = toBeChangedRow
                .With();

            nextRowBuilder.Remove(toBeRemovedTokenKey);

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

                nextRowBuilder.Insert(rememberTokenIndex + i, spaceWhiteSpaceToken);
            }

            if (tokenToInsertBetweenSplit is not null)
                nextRowBuilder.Insert(rememberTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 1, 
                    tokenToInsertBetweenSplit);
            
            var nextRowInstance = nextRowBuilder.Build();

            var nextRowList = focusedPlainTextEditorRecord.List
                .Replace(toBeChangedRow, nextRowInstance);

            return focusedPlainTextEditorRecord with
            {
                List = nextRowList,
                CurrentTokenIndex = rememberTokenIndex + toBeRemovedTokenIndexInPlainText!.Value + 
                    (tokenToInsertBetweenSplit is not null ? 1 : 0)
            };
        }
    }
}
