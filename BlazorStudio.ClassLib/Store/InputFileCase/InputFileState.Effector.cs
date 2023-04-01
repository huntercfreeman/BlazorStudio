using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Store.DialogCase;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Html;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

public partial record InputFileState
{
    private class Effector
    {
        private readonly ICommonComponentRenderers _commonComponentRenderers;

        public Effector(
            ICommonComponentRenderers commonComponentRenderers)
        {
            _commonComponentRenderers = commonComponentRenderers;
        }
        
        [EffectMethod]
        public Task HandleRequestInputFileStateFormAction(
            RequestInputFileStateFormAction requestInputFileStateFormAction,
            IDispatcher dispatcher)
        {
            if (_commonComponentRenderers.InputFileRendererType is not null)
            {
                dispatcher.Dispatch(
                    new StartInputFileStateFormAction(
                        requestInputFileStateFormAction));

                var inputFileDialog = new DialogRecord(
                    DialogFacts.InputFileDialogKey,
                    "Input File",
                    _commonComponentRenderers.InputFileRendererType,
                    null,
                    HtmlFacts.Classes.DIALOG_PADDING_0)
                {
                    IsResizable = true
                }; 
            
                dispatcher.Dispatch(
                    new DialogRecordsCollection.RegisterAction(
                        inputFileDialog));
            }

            return Task.CompletedTask;
        }
    }
}