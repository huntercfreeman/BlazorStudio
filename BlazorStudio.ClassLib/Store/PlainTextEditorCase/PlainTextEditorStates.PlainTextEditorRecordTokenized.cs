using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRecordTokenized(PlainTextEditorKey PlainTextEditorKey,
            SequenceKey SequenceKey,
            ImmutableList<IPlainTextEditorRow> Rows,
            int CurrentRowIndex,
            int CurrentTokenIndex,
            int CurrentColumnIndex,
            IAbsoluteFilePath? BackingAbsoluteFilePath,
            RichTextEditorOptions RichTextEditorOptions,
            bool IsReadonly = true,
            bool UseCarriageReturnNewLine = false)
        : PlainTextEditorRecordBase(PlainTextEditorKey,
            SequenceKey,
            Rows,
            CurrentRowIndex,
            CurrentTokenIndex,
            CurrentColumnIndex,
            RichTextEditorOptions,
            IsReadonly,
            UseCarriageReturnNewLine)
    {
        public PlainTextEditorRecordTokenized(PlainTextEditorKey plainTextEditorKey) : this(plainTextEditorKey,
            SequenceKey.NewSequenceKey(),
            ImmutableList<IPlainTextEditorRow>.Empty,
            CurrentRowIndex: 0,
            CurrentTokenIndex: 0,
            CurrentColumnIndex: 0,
            null,
            new RichTextEditorOptions())
        {
            Rows = Rows.Add(GetEmptyPlainTextEditorRow());
        }

        public override IPlainTextEditorRow CurrentPlainTextEditorRow => Rows[CurrentRowIndex];
        public override ITextToken CurrentTextToken => Rows[CurrentRowIndex].Tokens[CurrentTokenIndex];
        public override TextTokenKey CurrentTextTokenKey => CurrentTextToken.Key;
        public override PlainTextEditorKind PlainTextEditorKind => PlainTextEditorKind.Tokenized;

        public long CurrentPositionIndex { get; init; }
        public int LongestRowCharacterLength { get; init; }

        public override IAbsoluteFilePath? AbsoluteFilePath => BackingAbsoluteFilePath;

        public override string GetDocumentPlainText()
        {
            var builder = new StringBuilder();

            foreach (var row in Rows)
            {
                foreach (var token in row.Tokens)
                {
                    if (token.Key == Rows[0].Tokens[0].Key)
                    {
                        // Is first start of row so skip
                        // as it would insert a enter key stroke at start
                        // of document otherwise.

                        continue;
                    }

                    builder.Append(token.CopyText);
                }
            }

            return builder.ToString();
        }
        
        public override string GetSelectionPlainText()
        {
            if (SelectionSpan is null)
                return string.Empty;
            
            var builder = new StringBuilder();

            var runningTotalOfCharacters = 0;

            // The selection span when movement is to the left would make the logic confusing so standardize it.
            int indexSelectionLowerInclusiveBound = SelectionSpan.InclusiveStartingDocumentTextIndex;
            int indexSelectionUpperExclusiveBound = SelectionSpan.ExclusiveEndingDocumentTextIndex;

            if (SelectionSpan.SelectionDirection == SelectionDirection.Left)
            {
                indexSelectionLowerInclusiveBound = SelectionSpan.ExclusiveEndingDocumentTextIndex + 1;
                indexSelectionUpperExclusiveBound = SelectionSpan.InclusiveStartingDocumentTextIndex + 1;
            }

            foreach (var row in Rows)
            {
                foreach (var token in row.Tokens)
                {
                    if (token.Key == Rows[0].Tokens[0].Key)
                    {
                        // Is first start of row so skip
                        // as it would insert a enter key stroke at start
                        // of document otherwise.

                        continue;
                    }

                    var previousRunningTotalOfCharacters = runningTotalOfCharacters;
                    runningTotalOfCharacters += token.PlainText.Length;

                    if (indexSelectionLowerInclusiveBound <= runningTotalOfCharacters)
                    {
                        // Within selection

                        var plainText = token.PlainText;

                        int? lowerSubstringIndex = null;
                        int? upperSubstringIndex = null;

                        for (int i = 0; i < plainText.Length; i++)
                        {
                            // What characters actually are selected (is it the entire word?)
                            if (indexSelectionLowerInclusiveBound <= (previousRunningTotalOfCharacters + i))
                            {
                                if (indexSelectionUpperExclusiveBound > (previousRunningTotalOfCharacters + i))
                                {
                                    if (lowerSubstringIndex is null)
                                    {
                                        lowerSubstringIndex = i;
                                    }
                                    else 
                                    {
                                        upperSubstringIndex = i;
                                    }
                                }
                            }
                        }

                        var copyText = token.CopyText;

                        // The '\t' key is faked out as four spaces. This could cause the if statement to be true
                        if (lowerSubstringIndex > copyText.Length - 1)
                            lowerSubstringIndex = 0;

                        if (lowerSubstringIndex is null)
                            lowerSubstringIndex = 0;

                        if (upperSubstringIndex > copyText.Length - 1)
                            upperSubstringIndex = copyText.Length - 1;

                        if (upperSubstringIndex is null)
                            upperSubstringIndex = 0;

                        if (copyText.Length != 0)
                        {
                            var result = copyText.Substring(lowerSubstringIndex.Value,
                                upperSubstringIndex.Value - lowerSubstringIndex.Value + 1);

                            builder.Append(result);
                        }
                    }
                    
                    if (indexSelectionUpperExclusiveBound <= runningTotalOfCharacters)
                    {
                        // Went beyond the range of the selection so return early.
                        return builder.ToString();
                    }
                }
            }

            return builder.ToString();
        }

        public sealed override IPlainTextEditorRow GetEmptyPlainTextEditorRow()
        {
            return new PlainTextEditorRow(null);
        }

        public sealed override IPlainTextEditorRow GetWidthAndHeightTestPlainTextEditorRow()
        {
            var row = new PlainTextEditorRow(null);

            var contentBuilder = new StringBuilder();

            var repeatCount = 12;

            // 26 is the length of the 'HTML RENDERED' character content
            var letters = "abcdefghijklmnopqrstuvwxyz";
            // 10 is the length of the 'HTML RENDERED' character content
            var numbers = "0123456789";
            // 28 is the length of the 'HTML RENDERED' character content
            var punctuation = "`~-_=+\\|'\";:/?.>,<!@#$%^&*()".EscapeHtml();
            
            /*
             * width of character: 10.84375 when 1 repeat
             * width of character: 10.8359375 when 2 repeat
             * width of character: 10.8359375 when 6 repeat
             * width of character: 10.837239583333334 when 12 repeat
             * width of character: 10.836875 when 25 repeat
             * width of character: 10.8365625 when 50 repeat
             * width of character: 10.83734375 when 100 repeat
             */
            
            
            for (int i = 0; i < repeatCount; i++)
            {
                contentBuilder.Append(letters);
                contentBuilder.Append(numbers);
                contentBuilder.Append(punctuation);    
            }
            
            row = row with
            {
                Tokens = row.Tokens.Add(new DefaultTextToken() with
                {
                    // Added an extra 'q'
                    Content = contentBuilder.ToString()
                })
            };

            return row;
        }
    }
}