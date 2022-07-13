using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlainTextEditor.ClassLib.Keyboard;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private partial class StateMachine
    {
        public static PlainTextEditorRecord HandleMetaKey(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MetaKeys.BACKSPACE_KEY:
                    return HandleBackspaceKey(focusedPlainTextEditorRecord, keyDownEventRecord);
                default:
                   return focusedPlainTextEditorRecord;
            }
        }

        public static PlainTextEditorRecord HandleBackspaceKey(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            Console.WriteLine("HandleBackspaceKey");
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Default)
            {
                Console.WriteLine("HandleDefaultBackspace");
                return HandleDefaultBackspace(focusedPlainTextEditorRecord, keyDownEventRecord);
            }

            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.StartOfRow &&
                focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>().Array.Length > 1)
            {
                Console.WriteLine("MoveCurrentRowToEndOfPreviousRow");
                focusedPlainTextEditorRecord = MoveCurrentRowToEndOfPreviousRow(focusedPlainTextEditorRecord);
            }
            else
            {
                Console.WriteLine("RemoveCurrentToken");
                focusedPlainTextEditorRecord = RemoveCurrentToken(focusedPlainTextEditorRecord);
            }

            Console.WriteLine("MergeTokensIfApplicable");
            focusedPlainTextEditorRecord = MergeTokensIfApplicable(focusedPlainTextEditorRecord);

            Console.WriteLine("return");
            return focusedPlainTextEditorRecord;
        }
    }
}
