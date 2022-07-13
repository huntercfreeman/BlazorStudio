using Fluxor;
using PlainTextEditor.ClassLib.Sequence;
using PlainTextEditor.ClassLib.Store.KeyDownEventCase;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    public class PlainTextEditorStatesReducer
    {
        private readonly IState<PlainTextEditorStates> _plainTextEditorStatesWrap;

        public PlainTextEditorStatesReducer(IState<PlainTextEditorStates> plainTextEditorStatesWrap)
        {
            _plainTextEditorStatesWrap = plainTextEditorStatesWrap;
        }
        
        private readonly SemaphoreSlim _effectSemaphoreSlim = new(1, 1);
        private readonly ConcurrentQueue<Func<Task>> _concurrentQueueForEffects = new();

        private int _queuedEffectsCounter;

        [ReducerMethod]
        public static PlainTextEditorStates ReduceConstructPlainTextEditorAction(PlainTextEditorStates previousPlainTextEditorStates,
            ConstructPlainTextEditorRecordAction constructPlainTextEditorRecordAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = new 
                PlainTextEditorRecord(constructPlainTextEditorRecordAction.PlainTextEditorKey);

            nextPlainTextEditorMap[constructPlainTextEditorRecordAction.PlainTextEditorKey] = plainTextEditor;
            nextPlainTextEditorList.Add(constructPlainTextEditorRecordAction.PlainTextEditorKey);

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }
        
        [ReducerMethod]
        public static PlainTextEditorStates ReduceDeconstructPlainTextEditorRecordAction(PlainTextEditorStates previousPlainTextEditorStates,
            DeconstructPlainTextEditorRecordAction deconstructPlainTextEditorRecordAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            nextPlainTextEditorMap.Remove(deconstructPlainTextEditorRecordAction.PlainTextEditorKey);
            nextPlainTextEditorList.Remove(deconstructPlainTextEditorRecordAction.PlainTextEditorKey);

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }
        
        [EffectMethod]
        public async Task HandleKeyDownEventAction(KeyDownEventAction keyDownEventAction,
            IDispatcher dispatcher)
        {
            _queuedEffectsCounter++;

            try
            {
                _concurrentQueueForEffects.Enqueue(async () =>
                {
                    var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                    var startTimeUtc = DateTime.UtcNow;

                    //var delayInMiliseconds = 1;
                    //Console.WriteLine($"delayInMiliseconds: {delayInMiliseconds}");
                    //await Task.Delay(delayInMiliseconds);

                    var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                    var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                    var focusedPlainTextEditor = previousPlainTextEditorStates.Map[keyDownEventAction.FocusedPlainTextEditorKey]
                        as PlainTextEditorRecord;

                    if (focusedPlainTextEditor is null)
                        return;

                    var replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                        .HandleKeyDownEvent(focusedPlainTextEditor, keyDownEventAction.KeyDownEventRecord) with
                    {
                        SequenceKey = SequenceKey.NewSequenceKey()
                    };

                    nextPlainTextEditorMap[keyDownEventAction.FocusedPlainTextEditorKey] = replacementPlainTextEditor;

                    var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                    var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                    var endTimeUtc = DateTime.UtcNow;

                    var elapsedTimeSpan = endTimeUtc - startTimeUtc;

                    Console.WriteLine($"zMiliseconds: {elapsedTimeSpan.TotalMilliseconds}");
                    //Console.WriteLine($"_queuedEffectsCounter: {_queuedEffectsCounter}");

                    await Task.Delay(1);

                    _queuedEffectsCounter--;

                    dispatcher.Dispatch(
                        new SetPlainTextEditorStatesAction(
                            new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
                });

                await _effectSemaphoreSlim.WaitAsync();

                if(_concurrentQueueForEffects.TryDequeue(out var effect))
                    await effect.Invoke();
            }
            finally
            {
                _effectSemaphoreSlim.Release();
            }
        }
        
        [ReducerMethod]
        public static PlainTextEditorStates ReduceSetPlainTextEditorStatesAction(PlainTextEditorStates previousPlainTextEditorStates,
            SetPlainTextEditorStatesAction setPlainTextEditorStatesAction)
        {
            return setPlainTextEditorStatesAction.PlainTextEditorStates;
        }
        
        [ReducerMethod]
        public static PlainTextEditorStates ReducePlainTextEditorOnClickAction(PlainTextEditorStates previousPlainTextEditorStates,
            PlainTextEditorOnClickAction plainTextEditorOnClickAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);
            
            var focusedPlainTextEditor = previousPlainTextEditorStates.Map[plainTextEditorOnClickAction.FocusedPlainTextEditorKey]
                as PlainTextEditorRecord;

            if (focusedPlainTextEditor is null) 
                return previousPlainTextEditorStates;

            var replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                .HandleOnClickEvent(focusedPlainTextEditor, plainTextEditorOnClickAction) with
            {
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            nextPlainTextEditorMap[plainTextEditorOnClickAction.FocusedPlainTextEditorKey] = replacementPlainTextEditor;

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }
    }

    /// <summary>
    /// https://stackoverflow.com/questions/4228864/does-lock-guarantee-acquired-in-order-requested
    /// </summary>
    public class QueuedActions
    {
        private readonly object _internalSyncronizer = new object();
        private readonly ConcurrentQueue<Action> _actionsQueue = new ConcurrentQueue<Action>();

        public void Execute(Action action)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            _actionsQueue.Enqueue(action);

            lock (_internalSyncronizer)
            {
                Action nextAction;
                if (_actionsQueue.TryDequeue(out nextAction))
                {
                    nextAction.Invoke();
                }
                else
                {
                    throw new Exception("Something is wrong. How come there is nothing in the queue?");
                }
            }
        }
    }
}

