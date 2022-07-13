using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlainTextEditor.ClassLib.Keyboard;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private abstract record TextTokenBase : ITextToken
    {
        public abstract string PlainText { get; }
        public abstract TextTokenKind Kind { get; }
        public TextTokenKey Key { get; init; } = TextTokenKey.NewTextTokenKey();
        public int? IndexInPlainText { get; init; }
    }

    private record StartOfRowTextToken : TextTokenBase
    {
        public override string PlainText => "\n";
        public override TextTokenKind Kind => TextTokenKind.StartOfRow;
    }

    private record DefaultTextToken : TextTokenBase
    {
        // TODO: Immutable, efficient, updating of the _content string when user types.
        public string Content { get; init; }
        
        public override string PlainText => Content;
        public override TextTokenKind Kind => TextTokenKind.Default;
    }

    private record WhitespaceTextToken : TextTokenBase
    {
        // TODO: Immutable, efficient, updating of the _content string when user types.
        private string _content;

        public WhitespaceTextToken(KeyDownEventRecord keyDownEventRecord)
        {
            switch (keyDownEventRecord.Code)
            {
                case KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE:
                    WhitespaceKind = WhitespaceKind.Space;
                    _content = " ";
                    break;
                case KeyboardKeyFacts.WhitespaceKeys.TAB_CODE:
                    WhitespaceKind = WhitespaceKind.Tab;
                    _content = "    ";
                    break;
                default:
                    throw new ApplicationException(
                        $"The whitespace key: {keyDownEventRecord.Key} was not recognized.");
            }

            IndexInPlainText = _content.Length - 1;
        }
        
        public override string PlainText => _content;
        public override TextTokenKind Kind => TextTokenKind.Whitespace;
        
        public WhitespaceKind WhitespaceKind { get; }
    }
}
