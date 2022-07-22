using BlazorStudio.ClassLib.Keyboard;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private abstract record TextTokenBase : ITextToken
    {
        public abstract string PlainText { get; }
        public abstract string CopyText { get; }
        public abstract TextTokenKind Kind { get; }
        public TextTokenKey Key { get; init; } = TextTokenKey.NewTextTokenKey();
        public int? IndexInPlainText { get; init; }
    }

    private record StartOfRowTextToken(KeyDownEventRecord? KeyDownEventRecord) : TextTokenBase
    {
        public override string PlainText => "\n";
        public override string CopyText => IsCarriageReturn()
            ? "\r\n"
            : PlainText;

        public override TextTokenKind Kind => TextTokenKind.StartOfRow;

        public bool IsCarriageReturn()
        {
            if (KeyDownEventRecord is null)
            {
                return false;
            }

            return
                KeyDownEventRecord.Code == KeyboardKeyFacts.NewLineCodes.CARRIAGE_RETURN_NEW_LINE_CODE;
        }
    }

    private record DefaultTextToken : TextTokenBase
    {
        // TODO: Immutable, efficient, updating of the _content string when user types.
        public string Content { get; init; }
        
        public override string PlainText => Content;
        public override string CopyText => PlainText;
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
        
        public override string CopyText => WhitespaceKind == WhitespaceKind.Space
            ? PlainText
            : "\t";

        public override TextTokenKind Kind => TextTokenKind.Whitespace;
        
        public WhitespaceKind WhitespaceKind { get; }
    }
}
