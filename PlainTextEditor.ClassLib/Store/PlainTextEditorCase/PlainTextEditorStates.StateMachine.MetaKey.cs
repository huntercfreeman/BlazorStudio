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
            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.Default)
            {
                return HandleDefaultBackspace(focusedPlainTextEditorRecord, keyDownEventRecord);
            }

            if (focusedPlainTextEditorRecord.CurrentTextToken.Kind == TextTokenKind.StartOfRow &&
                focusedPlainTextEditorRecord.GetCurrentPlainTextEditorRowAs<PlainTextEditorRow>().List.Count > 1)
            {
                focusedPlainTextEditorRecord = MoveCurrentRowToEndOfPreviousRow(focusedPlainTextEditorRecord);
            }
            else
            {
                focusedPlainTextEditorRecord = RemoveCurrentToken(focusedPlainTextEditorRecord);
            }

            focusedPlainTextEditorRecord = MergeTokensIfApplicable(focusedPlainTextEditorRecord);

            return focusedPlainTextEditorRecord;
        }
    }
}
