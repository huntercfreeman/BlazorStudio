using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public class MyCPUIntensiveService
{
    public void OnKeyDown(KeyDownEventCase.KeyDownEventAction keyDownEventAction)
    {
        Console.WriteLine("before dispatcher.Dispatch(dispatcher);");
        //_dispatcher.Dispatch(keyDownEventAction);
        Console.WriteLine("after dispatcher.Dispatch(dispatcher);");
    }
}
