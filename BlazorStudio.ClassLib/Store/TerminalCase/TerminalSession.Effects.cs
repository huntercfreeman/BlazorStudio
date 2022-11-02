using System.Collections.Concurrent;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public partial record TerminalSession
{
    private class TerminalSessionsStateEffects
    {
        public record QueueTerminalCommandToExecuteAction(TerminalCommand TerminalCommand);

        private readonly ConcurrentQueue<TerminalCommand>
            _terminalCommandQueue = new ConcurrentQueue<TerminalCommand>();

        private readonly SemaphoreSlim _executeTerminalCommandSemaphoreSlim = new(1, 1);
        private readonly SemaphoreSlim _disposeExecuteTerminalCommandConsumerSemaphoreSlim = new(1, 1);

        /// <summary>
        /// I am unsure if many async Tasks pending has a performance detriment.
        /// <br/><br/>
        /// Therefore <see cref="QueueHandleEffectAsync"/> will make use of one async Task
        /// that acts as a 'producer-consumer' situation.
        /// </summary>
        private bool _hasExecuteTerminalCommandConsumer;

        private bool _isDisposingExecuteTerminalCommandConsumer;

        private async Task QueueHandleEffectAsync()
        {
            try
            {
                var shouldConstructConsumer = await _executeTerminalCommandSemaphoreSlim.WaitAsync(0);

                if (!shouldConstructConsumer)
                    return;

                _hasExecuteTerminalCommandConsumer = true;

                continueUsingCurrentConsumer:

                while (_terminalCommandQueue
                       .TryDequeue(out var fifoTerminalCommand))
                {
                    await fifoTerminalCommand.CommandFunc
                        .Invoke(fifoTerminalCommand);
                }

                // Prior to disposing the consumer ensure
                // there are no producers enqueueing terminal commands 
                var shouldDisposeConsumer = await _disposeExecuteTerminalCommandConsumerSemaphoreSlim
                    .WaitAsync(0);

                if (!shouldDisposeConsumer)
                {
                    // If there is a producer trying to enqueue a terminal command
                    // do not dispose of the current consumer but instead
                    // wait for the producer to enqueue the terminal command and
                    // then goto the while loop again
                    await _disposeExecuteTerminalCommandConsumerSemaphoreSlim.WaitAsync();
                    _disposeExecuteTerminalCommandConsumerSemaphoreSlim.Release();

                    goto continueUsingCurrentConsumer;
                }
            }
            finally
            {
                _hasExecuteTerminalCommandConsumer = false;

                _executeTerminalCommandSemaphoreSlim.Release();
                _disposeExecuteTerminalCommandConsumerSemaphoreSlim.Release();
            }
        }

        [EffectMethod]
        public async Task HandleEnqueueProcessOnTerminalEntryAction(
            QueueTerminalCommandToExecuteAction enqueueProcessOnTerminalEntryAction,
            IDispatcher dispatcher)
        {
            _terminalCommandQueue.Enqueue(
                enqueueProcessOnTerminalEntryAction.TerminalCommand);

            dispatcher.Dispatch(new RegisterTerminalResultAction(
                enqueueProcessOnTerminalEntryAction.TerminalCommand));

            bool hasActiveConsumer = false;
            bool activeConsumerIsDisposing = false;

            bool needReleaseExecuteTerminalCommandSemaphoreSlim = false;
            bool needReleaseDisposeExecuteTerminalCommandConsumerSemaphoreSlim = false;

            try
            {
                // If a consumer has control of the _executeTerminalCommandSemaphoreSlim
                // but is not controlling _disposeExecuteTerminalCommandConsumerSemaphoreSlim
                // then that consumer will do another iteration of the while loop
                // and 'consume' the TerminalCommand which was 'produced' in this method.
                hasActiveConsumer = !(await _executeTerminalCommandSemaphoreSlim
                    .WaitAsync(0));

                activeConsumerIsDisposing =
                    !(await _disposeExecuteTerminalCommandConsumerSemaphoreSlim
                        .WaitAsync(0));

                needReleaseExecuteTerminalCommandSemaphoreSlim =
                    !hasActiveConsumer;

                needReleaseDisposeExecuteTerminalCommandConsumerSemaphoreSlim =
                    !activeConsumerIsDisposing;

                if (!hasActiveConsumer ||
                    hasActiveConsumer && activeConsumerIsDisposing)
                {
                    if (needReleaseExecuteTerminalCommandSemaphoreSlim)
                    {
                        _executeTerminalCommandSemaphoreSlim.Release();
                        needReleaseExecuteTerminalCommandSemaphoreSlim = false;
                    }

                    if (needReleaseDisposeExecuteTerminalCommandConsumerSemaphoreSlim)
                    {
                        _disposeExecuteTerminalCommandConsumerSemaphoreSlim.Release();
                        needReleaseDisposeExecuteTerminalCommandConsumerSemaphoreSlim = false;
                    }

                    // Construct new consumer
                    await QueueHandleEffectAsync();
                }
            }
            finally
            {
                if (needReleaseExecuteTerminalCommandSemaphoreSlim)
                {
                    _executeTerminalCommandSemaphoreSlim.Release();
                    needReleaseExecuteTerminalCommandSemaphoreSlim = false;
                }

                if (needReleaseDisposeExecuteTerminalCommandConsumerSemaphoreSlim)
                {
                    _disposeExecuteTerminalCommandConsumerSemaphoreSlim.Release();
                    needReleaseDisposeExecuteTerminalCommandConsumerSemaphoreSlim = false;
                }
            }
        }

        [ReducerMethod]
        public static TerminalSessionsState ReduceSetWorkingDirectoryAbsoluteFilePathStringAction(
            TerminalSessionsState inTerminalSessionsState,
            TerminalSessionsState.SetWorkingDirectoryAbsoluteFilePathStringAction
                setWorkingDirectoryAbsoluteFilePathStringAction)
        {
            if (inTerminalSessionsState.TerminalSessionMap.TryGetValue(
                    setWorkingDirectoryAbsoluteFilePathStringAction.TerminalSessionKey,
                    out var terminalSession))
            {
                terminalSession = terminalSession with
                {
                    WorkingDirectoryAbsoluteFilePathString = setWorkingDirectoryAbsoluteFilePathStringAction
                        .WorkingDirectoryAbsoluteFilePathString
                };

                var nextTerminalSessionMap = inTerminalSessionsState.TerminalSessionMap
                    .SetItem(terminalSession.TerminalSessionKey, terminalSession);

                return inTerminalSessionsState with
                {
                    TerminalSessionMap = nextTerminalSessionMap
                };
            }

            return inTerminalSessionsState;
        }
    }
}