using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        // Given:
        //
        // 'z Bill'
        //   ^
        //    (Remove Whitespace)
        //
        // tokens: 'z' and 'Bill' must be
        // merged to make the token: 'zBill'
        public static async Task<PlainTextEditorRecordBase> MergeTokensIfApplicableAsync(PlainTextEditorRecordBase focusedPlainTextEditorRecord,
            CancellationToken cancellationToken)
        {
            var textTokenKind = focusedPlainTextEditorRecord.CurrentTextToken.Kind;
            
            if (textTokenKind != TextTokenKind.Default &&
                textTokenKind != TextTokenKind.Punctuation)
                return focusedPlainTextEditorRecord;
            
            var nextTokenTuple = await GetNextTokenTupleAsync(focusedPlainTextEditorRecord,
                cancellationToken);

            if (nextTokenTuple.token.Kind != textTokenKind ||
                nextTokenTuple.token.Key == focusedPlainTextEditorRecord.CurrentTextTokenKey)
            {
                return focusedPlainTextEditorRecord;
            }

            DefaultTextToken replacementToken;

            if (textTokenKind == TextTokenKind.Default)
            {
                replacementToken= new DefaultTextToken()
                {
                    Content = focusedPlainTextEditorRecord.CurrentTextToken.PlainText +
                              nextTokenTuple.token.PlainText,
                    IndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true)
                };
            }
            else
            {
                replacementToken= new PunctuationTextToken
                {
                    Content = focusedPlainTextEditorRecord.CurrentTextToken.PlainText +
                              nextTokenTuple.token.PlainText,
                    IndexInPlainText = focusedPlainTextEditorRecord.CurrentTextToken.GetIndexInPlainText(true)
                };
            }   

            var currentRow = focusedPlainTextEditorRecord
                .GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>();

            var replacementRow = currentRow with
            {
                Tokens = currentRow.Tokens
                    .Remove(nextTokenTuple.token)
                    .Remove(focusedPlainTextEditorRecord.CurrentTextToken)
                    .Insert(focusedPlainTextEditorRecord.CurrentTokenIndex, replacementToken),
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            var nextRowList = focusedPlainTextEditorRecord.Rows.Replace(currentRow,
                replacementRow);

            return focusedPlainTextEditorRecord with
            {
                Rows = nextRowList
            };
        }
    }
}
