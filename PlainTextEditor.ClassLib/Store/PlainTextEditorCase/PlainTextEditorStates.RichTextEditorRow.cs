using PlainTextEditor.ClassLib.Sequence;
using System.Collections.Immutable;
using PlainTextEditor.ClassLib.Keyboard;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRow(PlainTextEditorRowKey Key, 
        SequenceKey SequenceKey,
        ImmutableList<ITextToken> List)
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

            List = List.Add(startOfRowToken);
        }
    }
}
