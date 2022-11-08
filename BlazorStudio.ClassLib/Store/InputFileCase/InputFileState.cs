using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

[FeatureState]
public record InputFileState(
    int IndexInHistory,
    Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc,
    Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc,
    ImmutableArray<InputFilePattern> InputFilePatterns,
    InputFilePattern? SelectedInputFilePattern,
    string SearchQuery,
    string Message)
{
    private InputFileState() : this(
        0,
        _ => Task.CompletedTask,
        _ => Task.FromResult(false),
        ImmutableArray<InputFilePattern>.Empty,
        null,
        string.Empty,
        string.Empty) 
    {
    }

    public record RequestInputFileStateFormAction(
        string Message,
        Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc,
        Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc,
        ImmutableArray<InputFilePattern> InputFilePatterns);
    
    public record SetSelectedInputFilePatternAction(
        InputFilePattern InputFilePattern);
    
    public record SetSearchQueryAction(
        string SearchQuery);

    public record MoveBackwardsInHistoryAction;
    public record MoveForwardsInHistoryAction;
    public record OpenParentDirectoryAction;
    public record RefreshCurrentSelectionAction;

    public static bool CanMoveBackwardsInHistory(InputFileState inputFileState) => 
        inputFileState.IndexInHistory > 0;
    
    private class InputFileStateReducer
    {
        [ReducerMethod]
        public static InputFileState ReduceStartInputFileStateFormAction(
            InputFileState inInputFileState,
            InputFileStateEffects.StartInputFileStateFormAction startInputFileStateFormAction)
        {
            return inInputFileState with
            {
                SelectionIsValidFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.SelectionIsValidFunc,
                OnAfterSubmitFunc = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.OnAfterSubmitFunc,
                InputFilePatterns = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.InputFilePatterns,
                SelectedInputFilePattern = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.InputFilePatterns
                    .First(),
                Message = startInputFileStateFormAction
                    .RequestInputFileStateFormAction.Message
            };
        }
        
        [ReducerMethod]
        public static InputFileState ReduceSetSelectedInputFilePatternAction(
            InputFileState inInputFileState,
            SetSelectedInputFilePatternAction setSelectedInputFilePatternAction)
        {
            return inInputFileState with
            {
                SelectedInputFilePattern = 
                    setSelectedInputFilePatternAction.InputFilePattern
            };
        }

        [ReducerMethod]
        public static InputFileState ReduceMoveBackwardsInHistoryAction(
            InputFileState inInputFileState,
            MoveBackwardsInHistoryAction moveBackwardsInHistoryAction)
        {
            if (CanMoveBackwardsInHistory(inInputFileState))
            {
                return inInputFileState with
                {
                    IndexInHistory = inInputFileState.IndexInHistory - 
                                             1,
                };
            }
            
            return inInputFileState;
        }
    }
    
    private class InputFileStateEffects
    {
        private readonly ICommonComponentRenderers _commonComponentRenderers;

        public InputFileStateEffects(
            ICommonComponentRenderers commonComponentRenderers)
        {
            _commonComponentRenderers = commonComponentRenderers;
        }
        
        public record StartInputFileStateFormAction(
            RequestInputFileStateFormAction RequestInputFileStateFormAction);
        
        [EffectMethod]
        public Task HandleRequestInputFileStateFormAction(
            RequestInputFileStateFormAction requestInputFileStateFormAction,
            IDispatcher dispatcher)
        {
            dispatcher.Dispatch(
                new StartInputFileStateFormAction(
                    requestInputFileStateFormAction));

            var inputFileDialog = new DialogRecord(
                DialogFacts.InputFileDialogKey,
                "Input File",
                _commonComponentRenderers.InputFileRendererType,
                null); 
            
            dispatcher.Dispatch(
                new RegisterDialogRecordAction(
                    inputFileDialog));

            return Task.CompletedTask;
        }
    }
}