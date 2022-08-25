using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record HtmlTextEditorRecordTokenized(PlainTextEditorKey PlainTextEditorKey,
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
        public HtmlTextEditorRecordTokenized(PlainTextEditorKey plainTextEditorKey) : this(plainTextEditorKey,
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

        public sealed override IPlainTextEditorRow GetEmptyPlainTextEditorRow()
        {
            return new PlainTextEditorRow(null);
        }
        
        public sealed override IPlainTextEditorRow GetWidthAndHeightTestPlainTextEditorRow()
        {
            var row = new PlainTextEditorRow(null);

            row = row with
            {
                Tokens = row.Tokens.Add(new DefaultTextToken() with
                {
                    Content = "0"
                })
            };

            return row;
        }
    }
}