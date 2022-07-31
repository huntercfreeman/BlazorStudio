using System.Collections.Concurrent;
using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Virtualize;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    public class PlainTextEditorStatesEffect
    {
        private readonly IState<PlainTextEditorStates> _plainTextEditorStatesWrap;
        private readonly ConcurrentQueue<Func<Task>> _handleEffectQueue = new();
        private readonly SemaphoreSlim _executeHandleEffectSemaphoreSlim = new(1, 1);

        private PlainTextEditorStatesEffect(IState<PlainTextEditorStates> plainTextEditorStatesWrap)
        {
            _plainTextEditorStatesWrap = plainTextEditorStatesWrap;
        }

        private async Task QueueHandleEffectAsync(Func<Task> func)
        {
            _handleEffectQueue.Enqueue(func);

            try
            {
                await _executeHandleEffectSemaphoreSlim.WaitAsync();

                if (_handleEffectQueue.TryDequeue(out var fifoHandleEffect))
                {
                    await fifoHandleEffect!.Invoke();
                }
            }
            finally
            {
                _executeHandleEffectSemaphoreSlim.Release();
            }
        }

        [EffectMethod]
        public async Task HandleMemoryMappedFilePixelReadRequestAction(MemoryMappedFilePixelReadRequestAction memoryMappedFilePixelReadRequestAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var actionRequest = memoryMappedFilePixelReadRequestAction.VirtualizeCoordinateSystemMessage
                    .VirtualizeCoordinateSystemRequest;

                if (actionRequest is null ||
                    actionRequest.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var nextPlainTextEditorMap =
                    new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var plainTextEditor = previousPlainTextEditorStates
                        .Map[memoryMappedFilePixelReadRequestAction.PlainTextEditorKey]
                    as PlainTextEditorRecord;

                if (plainTextEditor?.FileHandle is null)
                    return;

                // TODO: The font-size style attribute does not equal the size of the div that encapsulates the singular character. Figure out EXACTLY these values based off the font-size instead of hard coding what developer tools says
                var heightOfEachRowInPixels = 27;
                var widthOfEachCharacterInPixels = 9.91;

                var startingRowIndex =
                    (int)(actionRequest.ScrollTopInPixels / heightOfEachRowInPixels);

                var requestRowCount = (int)(actionRequest.ViewportHeightInPixels / heightOfEachRowInPixels);

                var startingCharacterIndex = (int)(actionRequest.ScrollLeftInPixels / widthOfEachCharacterInPixels);

                var requestCharacterCount = (int)(actionRequest.ViewportWidthInPixels / widthOfEachCharacterInPixels);

                var readRequest = new FileHandleReadRequest(
                    startingRowIndex,
                    startingCharacterIndex,
                    requestRowCount,
                    requestCharacterCount,
                    actionRequest.CancellationToken);

                var contentRows = await plainTextEditor.FileHandle
                    .ReadAsync(readRequest);

                if (contentRows is null)
                    return;

                var replacementPlainTextEditor = plainTextEditor with
                {
                    SequenceKey = SequenceKey.NewSequenceKey(),
                    FileHandleReadRequest = readRequest,
                    List = new IPlainTextEditorRow[]
                    {
                        plainTextEditor.GetEmptyPlainTextEditorRow()
                    }.ToImmutableList(),
                    CurrentRowIndex = 0,
                    CurrentTokenIndex = 0
                };

                var allEnterKeysAreCarriageReturnNewLine = true;
                var seenEnterKey = false;
                var previousCharacterWasCarriageReturn = false;

                var currentRowCharacterLength = 0;
                var longestRowCharacterLength = 0;

                string MutateIfPreviousCharacterWasCarriageReturn()
                {
                    longestRowCharacterLength = currentRowCharacterLength > longestRowCharacterLength
                        ? currentRowCharacterLength
                        : longestRowCharacterLength;

                    currentRowCharacterLength = 0;

                    seenEnterKey = true;

                    if (!previousCharacterWasCarriageReturn)
                    {
                        allEnterKeysAreCarriageReturnNewLine = false;
                    }

                    return previousCharacterWasCarriageReturn
                        ? KeyboardKeyFacts.WhitespaceKeys.CARRIAGE_RETURN_NEW_LINE_CODE
                        : KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE;
                }

                for (var index = 0; index < contentRows.Count; index++)
                {
                    var row = contentRows[index];

                    foreach (var character in row)
                    {
                        if (character == '\r')
                        {
                            previousCharacterWasCarriageReturn = true;
                            continue;
                        }

                        currentRowCharacterLength++;

                        var code = character switch
                        {
                            '\t' => KeyboardKeyFacts.WhitespaceKeys.TAB_CODE,
                            ' ' => KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE,
                            '\n' => MutateIfPreviousCharacterWasCarriageReturn(),
                            _ => character.ToString()
                        };

                        if (character == '\n')
                        {
                            // TODO: I think ignoring this is correct but unsure
                            continue;
                        }

                        var keyDown = new KeyDownEventAction(replacementPlainTextEditor.PlainTextEditorKey,
                            new KeyDownEventRecord(
                                character.ToString(),
                                code,
                                false,
                                false,
                                false,
                                IsForced: true
                            )
                        );

                        replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                                .HandleKeyDownEvent(replacementPlainTextEditor, keyDown.KeyDownEventRecord) with
                            {
                                SequenceKey = SequenceKey.NewSequenceKey()
                            };

                        previousCharacterWasCarriageReturn = false;
                    }

                    if ((index != contentRows.Count - 1) || (row.LastOrDefault() == '\n'))
                    {
                        var newLineCode = MutateIfPreviousCharacterWasCarriageReturn();

                        var forceNewLine = new KeyDownEventRecord(
                            newLineCode,
                            newLineCode,
                            false,
                            false,
                            false,
                            IsForced: true);

                        replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                                .HandleKeyDownEvent(replacementPlainTextEditor, forceNewLine) with
                            {
                                SequenceKey = SequenceKey.NewSequenceKey(),
                            };
                    }
                }

                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    RowIndexOffset = readRequest.RowIndexOffset
                };

                var actualWidthOfResult = widthOfEachCharacterInPixels * requestCharacterCount;

                var actualHeightOfResult = heightOfEachRowInPixels * requestRowCount;

                var totalWidth = widthOfEachCharacterInPixels *
                                 replacementPlainTextEditor.FileHandle.VirtualCharacterLengthOfLongestRow;

                var totalHeight = heightOfEachRowInPixels *
                                  replacementPlainTextEditor.FileHandle.VirtualRowCount;

                var items = replacementPlainTextEditor.List
                    .Select((row, index) => (index, row));

                var result = new VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(
                    items,
                    items.Select(x => (object)x),
                    actualWidthOfResult,
                    actualHeightOfResult,
                    totalWidth,
                    totalHeight);

                var message = memoryMappedFilePixelReadRequestAction.VirtualizeCoordinateSystemMessage with
                {
                    VirtualizeCoordinateSystemResult = result
                };

                var resultingPlainTextEditor = replacementPlainTextEditor with
                {
                    LongestRowCharacterLength =
                    (int)replacementPlainTextEditor.FileHandle.VirtualCharacterLengthOfLongestRow,
                    RowIndexOffset = startingRowIndex,
                    VirtualizeCoordinateSystemMessage = message
                };

                nextPlainTextEditorMap[memoryMappedFilePixelReadRequestAction.PlainTextEditorKey] =
                    resultingPlainTextEditor;

                var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                if (actionRequest.CancellationToken.IsCancellationRequested)
                    return;

                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
            });
        }

        [EffectMethod]
        public async Task HandleKeyDownEventAction(KeyDownEventAction keyDownEventAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var plainTextEditor = previousPlainTextEditorStates.Map[keyDownEventAction.PlainTextEditorKey]
                    as PlainTextEditorRecord;

                if (plainTextEditor is null)
                    return;

                var overrideKeyDownEventRecord = keyDownEventAction.KeyDownEventRecord;

                if (keyDownEventAction.KeyDownEventRecord.Code == KeyboardKeyFacts.NewLineCodes.ENTER_CODE &&
                    plainTextEditor.UseCarriageReturnNewLine)
                {
                    overrideKeyDownEventRecord = keyDownEventAction.KeyDownEventRecord with
                    {
                        Code = KeyboardKeyFacts.NewLineCodes.CARRIAGE_RETURN_NEW_LINE_CODE
                    };
                }

                var replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                    .HandleKeyDownEvent(plainTextEditor, overrideKeyDownEventRecord) with
                {
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                if (replacementPlainTextEditor.VirtualizeCoordinateSystemMessage.VirtualizeCoordinateSystemResult is null)
                {
                    throw new ApplicationException(
                        $"{nameof(replacementPlainTextEditor.VirtualizeCoordinateSystemMessage.VirtualizeCoordinateSystemResult)} was null.");
                }

                var previousResult = (VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>)
                    replacementPlainTextEditor.VirtualizeCoordinateSystemMessage.VirtualizeCoordinateSystemResult;

                var items = replacementPlainTextEditor.List
                    .Select((row, index) => (index, row))
                    .ToList();

                var virtualizeCoordinateSystemResult = previousResult with
                {
                    ItemsWithType = items,
                    ItemsUntyped = items.Select(x => (object)x)
                };

                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    SequenceKey = SequenceKey.NewSequenceKey(),
                    VirtualizeCoordinateSystemMessage = replacementPlainTextEditor.VirtualizeCoordinateSystemMessage with
                    {
                        VirtualizeCoordinateSystemResult = virtualizeCoordinateSystemResult
                    }
                };

                nextPlainTextEditorMap[keyDownEventAction.PlainTextEditorKey] = replacementPlainTextEditor;

                var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
            });
        }

        [EffectMethod]
        public async Task HandlePlainTextEditorOnClickAction(PlainTextEditorOnClickAction plainTextEditorOnClickAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var plainTextEditor = previousPlainTextEditorStates.Map[plainTextEditorOnClickAction.PlainTextEditorKey]
                    as PlainTextEditorRecord;

                if (plainTextEditor is null)
                    return;

                var replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                    .HandleOnClickEvent(plainTextEditor, plainTextEditorOnClickAction) with
                {
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                if (replacementPlainTextEditor.VirtualizeCoordinateSystemMessage.VirtualizeCoordinateSystemResult is null)
                {
                    throw new ApplicationException(
                        $"{nameof(replacementPlainTextEditor.VirtualizeCoordinateSystemMessage.VirtualizeCoordinateSystemResult)} was null.");
                }

                var previousResult = (VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>)
                    replacementPlainTextEditor.VirtualizeCoordinateSystemMessage.VirtualizeCoordinateSystemResult;

                var items = replacementPlainTextEditor.List
                    .Select((row, index) => (index, row))
                    .ToList();

                var virtualizeCoordinateSystemResult = previousResult with
                {
                    ItemsWithType = items,
                    ItemsUntyped = items.Select(x => (object)x)
                };

                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    SequenceKey = SequenceKey.NewSequenceKey(),
                    VirtualizeCoordinateSystemMessage = replacementPlainTextEditor.VirtualizeCoordinateSystemMessage with
                    {
                        VirtualizeCoordinateSystemResult = virtualizeCoordinateSystemResult
                    }
                };

                nextPlainTextEditorMap[plainTextEditorOnClickAction.PlainTextEditorKey] = replacementPlainTextEditor;

                var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
            });
        }
        
        [EffectMethod]
        public async Task HandleSetIsReadonlyAction(SetIsReadonlyAction setIsReadonlyAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var plainTextEditor = previousPlainTextEditorStates.Map[setIsReadonlyAction.PlainTextEditorKey]
                    as PlainTextEditorRecord;

                if (plainTextEditor is null)
                    return;

                var replacementPlainTextEditor = plainTextEditor with
                {
                    SequenceKey = SequenceKey.NewSequenceKey(),
                    IsReadonly = setIsReadonlyAction.IsReadonly
                };

                nextPlainTextEditorMap[setIsReadonlyAction.PlainTextEditorKey] = replacementPlainTextEditor;

                var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
            });
        }

        [EffectMethod]
        public async Task HandleConstructMemoryMappedFilePlainTextEditorRecordAction(ConstructMemoryMappedFilePlainTextEditorRecordAction constructMemoryMappedFilePlainTextEditorRecordAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var anEditorIsAlreadyOpenedForTheFile = previousPlainTextEditorStates.Map.Any(x =>
                    (x.Value.FileHandle?.AbsoluteFilePath.GetAbsoluteFilePathString() ?? string.Empty) ==
                    constructMemoryMappedFilePlainTextEditorRecordAction.AbsoluteFilePath.GetAbsoluteFilePathString());

                if (anEditorIsAlreadyOpenedForTheFile)
                    return;

                var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var fileHandle = constructMemoryMappedFilePlainTextEditorRecordAction.FileSystemProvider
                    .Open(constructMemoryMappedFilePlainTextEditorRecordAction.AbsoluteFilePath);

                var plainTextEditor = new
                    PlainTextEditorRecord(constructMemoryMappedFilePlainTextEditorRecordAction.PlainTextEditorKey)
                    {
                        FileHandle = fileHandle
                    };

                nextPlainTextEditorMap[constructMemoryMappedFilePlainTextEditorRecordAction.PlainTextEditorKey] = plainTextEditor;
                nextPlainTextEditorList.Add(constructMemoryMappedFilePlainTextEditorRecordAction.PlainTextEditorKey);

                var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
            });
        }

        [EffectMethod]
        public async Task HandleDeconstructPlainTextEditorRecordAction(DeconstructPlainTextEditorRecordAction deconstructPlainTextEditorRecordAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var plainTextEditor = previousPlainTextEditorStates
                        .Map[deconstructPlainTextEditorRecordAction.PlainTextEditorKey]
                    as PlainTextEditorRecord;

                nextPlainTextEditorMap.Remove(deconstructPlainTextEditorRecordAction.PlainTextEditorKey);
                nextPlainTextEditorList.Remove(deconstructPlainTextEditorRecordAction.PlainTextEditorKey);

                if (plainTextEditor?.FileHandle is not null)
                    plainTextEditor.FileHandle.Dispose();

                var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
            });
        }
    }
}

