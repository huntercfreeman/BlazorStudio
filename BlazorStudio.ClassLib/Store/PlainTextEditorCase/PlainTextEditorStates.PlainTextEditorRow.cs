using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRow(PlainTextEditorRowKey Key, 
        SequenceKey SequenceKey,
        ImmutableList<ITextToken> Tokens)
            : IPlainTextEditorRow
    {
        public PlainTextEditorRow(KeyDownEventRecord? keyDownEventRecord) : this(PlainTextEditorRowKey.NewPlainTextEditorRowKey(), 
            SequenceKey.NewSequenceKey(),
            ImmutableList<ITextToken>.Empty)
        {
            var startOfRowToken = new StartOfRowTextToken(keyDownEventRecord)
            {
                IndexInPlainText = 0
            };

            Tokens = Tokens.Add(startOfRowToken);
        }
    }
}
