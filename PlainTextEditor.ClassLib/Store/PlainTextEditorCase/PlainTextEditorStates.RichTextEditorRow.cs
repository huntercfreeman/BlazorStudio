using PlainTextEditor.ClassLib.Sequence;
using System.Collections.Immutable;
using PlainTextEditor.ClassLib.Keyboard;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRow(PlainTextEditorRowKey Key, 
        SequenceKey SequenceKey,
        ImmutableDictionary<TextTokenKey, ITextToken> Map,
        ImmutableArray<TextTokenKey> Array)
            : IPlainTextEditorRow
    {
        public PlainTextEditorRow() : this(PlainTextEditorRowKey.NewPlainTextEditorRowKey(), 
            SequenceKey.NewSequenceKey(),
            new Dictionary<TextTokenKey, ITextToken>().ToImmutableDictionary(),
            new TextTokenKey[0].ToImmutableArray())
        {
            var startOfRowToken = new StartOfRowTextToken(null)
            {
                IndexInPlainText = 0
            };

            Map = new Dictionary<TextTokenKey, ITextToken>
            {
                { 
                    startOfRowToken.Key, 
                    startOfRowToken
                }
            }.ToImmutableDictionary();

            Array = new TextTokenKey[]
            {
                startOfRowToken.Key
            }.ToImmutableArray();
        }
        
        public PlainTextEditorRow(KeyDownEventRecord keyDownEventRecord) : this(PlainTextEditorRowKey.NewPlainTextEditorRowKey(),
            SequenceKey.NewSequenceKey(),
            new Dictionary<TextTokenKey, ITextToken>().ToImmutableDictionary(),
            new TextTokenKey[0].ToImmutableArray())
        {
            var startOfRowToken = new StartOfRowTextToken(keyDownEventRecord)
            {
                IndexInPlainText = 0
            };

            Map = new Dictionary<TextTokenKey, ITextToken>
            {
                { 
                    startOfRowToken.Key, 
                    startOfRowToken
                }
            }.ToImmutableDictionary();

            Array = new TextTokenKey[]
            {
                startOfRowToken.Key
            }.ToImmutableArray();
        }

        public IPlainTextEditorRowBuilder With()
        {
            return new PlainTextEditorRowBuilder(this);
        }
        
        private class PlainTextEditorRowBuilder : IPlainTextEditorRowBuilder
        {
            public PlainTextEditorRowBuilder()
            {
                
            }

            public PlainTextEditorRowBuilder(PlainTextEditorRow plainTextEditorRow)
            {
                Key = plainTextEditorRow.Key;
                Map = new(plainTextEditorRow.Map);
                List = new(plainTextEditorRow.Array);
            }
            
            private PlainTextEditorRowKey Key { get; } = PlainTextEditorRowKey.NewPlainTextEditorRowKey();
            private Dictionary<TextTokenKey, ITextToken> Map { get; } = new();  
            private List<TextTokenKey> List { get; } = new();

            public IPlainTextEditorRowBuilder Add(ITextToken token)
            {
                Map.Add(token.Key, token);
                List.Add(token.Key);

                return this;
            }
            
            public IPlainTextEditorRowBuilder Insert(int index, ITextToken token)
            {
                Map.Add(token.Key, token);
                List.Insert(index, token.Key);

                return this;
            }

            public IPlainTextEditorRowBuilder Remove(TextTokenKey textTokenKey)
            {
                Map.Remove(textTokenKey);
                List.Remove(textTokenKey);

                return this;
            }

            public IPlainTextEditorRowBuilder ReplaceMapValue(ITextToken token)
            {
                Map[token.Key] = token;

                return this;
            }
            
            public IPlainTextEditorRow Build()
            {
                return new PlainTextEditorRow(Key,
                    SequenceKey.NewSequenceKey(),
                    Map.ToImmutableDictionary(),
                    List.ToImmutableArray());
            }
        }
    }
}
