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
    private record PlainTextEditorRecordMemoryMappedFile(PlainTextEditorKey PlainTextEditorKey,
            SequenceKey SequenceKey,
            ImmutableList<IPlainTextEditorRow> Rows,
            int CurrentRowIndex,
            int CurrentTokenIndex,
            int CurrentColumnIndex,
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
        public PlainTextEditorRecordMemoryMappedFile(PlainTextEditorKey plainTextEditorKey) : this(plainTextEditorKey,
            SequenceKey.NewSequenceKey(),
            ImmutableList<IPlainTextEditorRow>.Empty,
            CurrentRowIndex: 0,
            CurrentTokenIndex: 0,
            CurrentColumnIndex: 0,
            new RichTextEditorOptions())
        {
            Rows = Rows.Add(GetEmptyPlainTextEditorRow());
        }

        public override IPlainTextEditorRow CurrentPlainTextEditorRow => Rows[CurrentRowIndex];
        public override ITextToken CurrentTextToken => Rows[CurrentRowIndex].Tokens[CurrentTokenIndex];
        public override TextTokenKey CurrentTextTokenKey => CurrentTextToken.Key;
        public override PlainTextEditorKind PlainTextEditorKind => PlainTextEditorKind.MemoryMappedFile;

        public int LongestRowCharacterLength { get; init; }

        public override IAbsoluteFilePath? AbsoluteFilePath => FileHandle.AbsoluteFilePath;

        public override long CurrentPositionIndex =>
            FileHandle.VirtualCharacterIndexMarkerForStartOfARow[RowIndexOffset + CurrentRowIndex]
                + CharacterColumnIndexOffset + CurrentCharacterColumnIndex;

        public override string GetPlainText()
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
    }
}