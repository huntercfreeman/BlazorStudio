using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluxor;
using PlainTextEditor.ClassLib.Sequence;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRecord(PlainTextEditorKey PlainTextEditorKey,
            SequenceKey SequenceKey,
            ImmutableList<IPlainTextEditorRow> List,
            int CurrentRowIndex,
            int CurrentTokenIndex,
            RichTextEditorOptions RichTextEditorOptions)
        : IPlainTextEditor
    {
        public PlainTextEditorRecord(PlainTextEditorKey plainTextEditorKey) : this(plainTextEditorKey,
            SequenceKey.NewSequenceKey(),
            ImmutableList<IPlainTextEditorRow>.Empty,
            CurrentRowIndex: 0,
            CurrentTokenIndex: 0,
            new RichTextEditorOptions())
        {
            var startingRow = new PlainTextEditorRow(null);

            List = List.Add(startingRow);
        }

        public IPlainTextEditorRow CurrentPlainTextEditorRow => List[CurrentRowIndex];

        public TextTokenKey CurrentTextTokenKey => CurrentPlainTextEditorRow.Array[CurrentTokenIndex];
        public ITextToken CurrentTextToken => CurrentPlainTextEditorRow.Map[CurrentTextTokenKey];

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
                foreach (var tokenKey in row.Array)
                {
                    if (tokenKey == List[0].Array[0])
                    {
                        // Is first start of row so skip
                        // as it would insert a enter key stroke at start
                        // of document otherwise.

                        continue;
                    }

                    var currentToken = row.Map[tokenKey];

                    builder.Append(currentToken.CopyText);
                }
            }

            return builder.ToString();
        }
    }
}