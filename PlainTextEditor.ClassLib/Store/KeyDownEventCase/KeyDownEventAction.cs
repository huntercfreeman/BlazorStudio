using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using PlainTextEditor.ClassLib.Keyboard;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.ClassLib.Store.KeyDownEventCase;

public record KeyDownEventAction(PlainTextEditorKey FocusedPlainTextEditorKey, 
    KeyDownEventRecord KeyDownEventRecord);
