using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRecord(PlainTextEditorKey PlainTextEditorKey,
            SequenceKey SequenceKey,
            ImmutableList<IPlainTextEditorRow> Rows,
            int CurrentRowIndex,
            int CurrentTokenIndex,
            int CurrentColumnIndex,
            IFileHandle? FileHandle,
            RichTextEditorOptions RichTextEditorOptions,
            bool IsReadonly = true,
            bool UseCarriageReturnNewLine = false)
        : IPlainTextEditor
    {
        public PlainTextEditorRecord(PlainTextEditorKey plainTextEditorKey) : this(plainTextEditorKey,
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

        public IPlainTextEditorRow CurrentPlainTextEditorRow => Rows[CurrentRowIndex];

        public TextTokenKey CurrentTextTokenKey => CurrentPlainTextEditorRow.Tokens[CurrentTokenIndex].Key;
        public ITextToken CurrentTextToken => CurrentPlainTextEditorRow.Tokens[CurrentTokenIndex];
        public int LongestRowCharacterLength { get; init; }
        public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get; init; }
        public FileHandleReadRequest FileHandleReadRequest { get; init; }
        public int RowIndexOffset { get; init; }
        public int CharacterIndexOffsetRelativeToRow { get; init; }
        public int CurrentCharacterColumnIndex { get; init; }
        public int CurrentPositionIndex { get; init; }
        public int PreviouslySetCharacterColumnIndex { get; init; }
        public int CharacterColumnIndexOffset { get; init; }

        public T GetCurrentTextTokenAs<T>()
            where T : class
        {
            return CurrentTextToken as T
                   ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }

        /// <summary>
        /// TODO: Remove this and in its place use <see cref="ConvertIPlainTextEditorRowAs"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public T GetCurrentPlainTextEditorRowAs<T>()
            where T : class
        {
            return CurrentPlainTextEditorRow as T
                   ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }

        public T ConvertIPlainTextEditorRowAs<T>(IPlainTextEditorRow plainTextEditorRow)
            where T : class
        {
            return plainTextEditorRow as T
                   ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }

        public string GetPlainText()
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
        
        public IPlainTextEditorRow GetEmptyPlainTextEditorRow()
        {
            return new PlainTextEditorRow(null);
        }
    }
}