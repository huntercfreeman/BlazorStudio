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
        public static PlainTextEditorRecord HandleWhitespace(PlainTextEditorRecord focusedPlainTextEditorRecord,
            KeyDownEventRecord keyDownEventRecord)
        {
            var rememberToken = focusedPlainTextEditorRecord
                    .GetCurrentTextTokenAs<TextTokenBase>();

            if (rememberToken.IndexInPlainText!.Value != rememberToken.PlainText.Length - 1)
            {
                if (KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE == keyDownEventRecord.Code)
                {
                    focusedPlainTextEditorRecord = SplitCurrentToken(
                        focusedPlainTextEditorRecord,
                        null
                    );

                    return InsertNewLine(focusedPlainTextEditorRecord);
                }

                return SplitCurrentToken(
                    focusedPlainTextEditorRecord,
                        new WhitespaceTextToken(keyDownEventRecord)
                );
            }
            else
            {
                if (KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE == keyDownEventRecord.Code)
                {
                    return InsertNewLine(focusedPlainTextEditorRecord);
                }

                return InsertNewCurrentTokenAfterCurrentPosition(focusedPlainTextEditorRecord,
                    new WhitespaceTextToken(keyDownEventRecord));
            }
        }
    }
}
