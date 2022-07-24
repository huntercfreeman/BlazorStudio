using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRecord(PlainTextEditorKey PlainTextEditorKey,
            SequenceKey SequenceKey,
            ImmutableList<IPlainTextEditorRow> List,
            int CurrentRowIndex,
            int CurrentTokenIndex,
            IFileCoordinateGrid? FileCoordinateGrid,
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
            null,
            new RichTextEditorOptions())
        {
            List = List.Add(GetEmptyPlainTextEditorRow());
        }

        public IPlainTextEditorRow CurrentPlainTextEditorRow => List[CurrentRowIndex];

        public TextTokenKey CurrentTextTokenKey => CurrentPlainTextEditorRow.List[CurrentTokenIndex].Key;
        public ITextToken CurrentTextToken => CurrentPlainTextEditorRow.List[CurrentTokenIndex];
        public int LongestRowCharacterLength { get; init; }
        public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get; init; }
        public FileCoordinateGridRequest FileCoordinateGridRequest { get; init; }
        public int RowIndexOffset { get; init; }

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

            foreach (var row in List)
            {
                foreach (var token in row.List)
                {
                    if (token.Key == List[0].List[0].Key)
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