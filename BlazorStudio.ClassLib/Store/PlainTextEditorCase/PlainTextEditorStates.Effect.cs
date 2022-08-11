using System.Collections.Concurrent;
using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.Virtualize;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    public class PlainTextEditorStatesEffect
    {
        private readonly IState<PlainTextEditorStates> _plainTextEditorStatesWrap;
        private readonly IState<SolutionState> _solutionStateWrap;
        private readonly ConcurrentQueue<Func<Task>> _handleEffectQueue = new();
        private readonly SemaphoreSlim _executeHandleEffectSemaphoreSlim = new(1, 1);
        private Task<ITaskModel> _updateTokenSemanticDescriptions;

        public PlainTextEditorStatesEffect(IState<PlainTextEditorStates> plainTextEditorStatesWrap,
            IState<SolutionState> solutionStateWrap)
        {
            _plainTextEditorStatesWrap = plainTextEditorStatesWrap;
            _solutionStateWrap = solutionStateWrap;
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

        /// <summary>
        /// Extremely large files may need this logic as perhaps it is not a good idea
        /// to hold them in memory.
        /// </summary>
        [EffectMethod]
        public async Task HandleMemoryMappedFilePixelReadRequestAction(PlainTextEditorPixelReadRequestAction plainTextEditorPixelReadRequestAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var actionRequest = plainTextEditorPixelReadRequestAction.VirtualizeCoordinateSystemMessage
                    .VirtualizeCoordinateSystemRequest;

                if (actionRequest is null ||
                    actionRequest.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var nextPlainTextEditorMap =
                    new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var plainTextEditorUnknown = previousPlainTextEditorStates
                    .Map[plainTextEditorPixelReadRequestAction.PlainTextEditorKey];

                var heightOfEachRowInPixels = plainTextEditorUnknown.RichTextEditorOptions.HeightOfARowInPixels;
                var widthOfEachCharacterInPixels = plainTextEditorUnknown.RichTextEditorOptions.WidthOfACharacterInPixels;

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

                if (plainTextEditorUnknown.PlainTextEditorKind == PlainTextEditorKind.Tokenized)
                {
                    var tokenizedEditor = plainTextEditorUnknown as PlainTextEditorRecordTokenized;

                    var rowsResult = tokenizedEditor.Rows
                        .Select((row, index) => (index, row))
                        .Skip(startingRowIndex)
                        .Take(requestRowCount);

                    List<(int, IPlainTextEditorRow)> horizontallyVirtualizedResult = new List<(int, IPlainTextEditorRow)>();

                    foreach (var row in rowsResult)
                    {
                        var conciseRow = new PlainTextEditorRow(row.row.Key,
                            SequenceKey.NewSequenceKey(),
                            row.row.Tokens
                                .Skip(startingCharacterIndex)
                                .Take(requestCharacterCount)
                                .ToImmutableList());

                        horizontallyVirtualizedResult.Add((row.index, conciseRow));
                    }

                    var result = new VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(
                        horizontallyVirtualizedResult,
                        horizontallyVirtualizedResult.Select(x => (object)x),
                        horizontallyVirtualizedResult.Max(x => x.Item2.Tokens.Count) * widthOfEachCharacterInPixels,
                        horizontallyVirtualizedResult.Count * heightOfEachRowInPixels,
                        tokenizedEditor.Rows.Max(x => x.Tokens.Count) * widthOfEachCharacterInPixels,
                        tokenizedEditor.Rows.Count * heightOfEachRowInPixels);

                    var message = plainTextEditorPixelReadRequestAction.VirtualizeCoordinateSystemMessage with
                    {
                        VirtualizeCoordinateSystemResult = result
                    };

                    var resultingPlainTextEditor = tokenizedEditor with
                    {
                        VirtualizeCoordinateSystemMessage = message,
                    };

                    nextPlainTextEditorMap[plainTextEditorPixelReadRequestAction.PlainTextEditorKey] =
                        resultingPlainTextEditor;

                    var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                    var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                    if (actionRequest.CancellationToken.IsCancellationRequested)
                        return;

                    dispatcher.Dispatch(new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap, nextImmutableArray)));
                }
                else
                {
                    var plainTextEditor = plainTextEditorUnknown
                        as PlainTextEditorRecordMemoryMappedFile;

                    if (plainTextEditor?.FileHandle is null)
                        return;

                    var contentRows = await plainTextEditor.FileHandle
                        .ReadAsync(readRequest);

                    if (contentRows is null)
                        return;

                    var replacementPlainTextEditor = plainTextEditor with
                    {
                        SequenceKey = SequenceKey.NewSequenceKey(),
                        FileHandleReadRequest = readRequest,
                        Rows = new IPlainTextEditorRow[]
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

                            var keyDownRecord = new KeyDownEventRecord(
                                character.ToString(),
                                code,
                                false,
                                false,
                                false,
                                IsForced: true
                            );

                            var resultPlainTextEditorRecord = await PlainTextEditorStates.StateMachine
                                .HandleKeyDownEventAsync(replacementPlainTextEditor,
                                    keyDownRecord,
                                    plainTextEditorPixelReadRequestAction.VirtualizeCoordinateSystemMessage
                                        .VirtualizeCoordinateSystemRequest.CancellationToken);

                            replacementPlainTextEditor =
                                ((PlainTextEditorRecordMemoryMappedFile)resultPlainTextEditorRecord) with
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

                            var newLinedPlainTextEditorRecord = await PlainTextEditorStates.StateMachine
                                .HandleKeyDownEventAsync(replacementPlainTextEditor,
                                    forceNewLine,
                                    plainTextEditorPixelReadRequestAction.VirtualizeCoordinateSystemMessage
                                        .VirtualizeCoordinateSystemRequest.CancellationToken);

                            replacementPlainTextEditor =
                                ((PlainTextEditorRecordMemoryMappedFile)newLinedPlainTextEditorRecord) with
                                {
                                    SequenceKey = SequenceKey.NewSequenceKey()
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

                    var items = replacementPlainTextEditor.Rows
                        .Select((row, index) => (index, row));

                    var result =
                        new VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(
                            items,
                            items.Select(x => (object)x),
                            actualWidthOfResult,
                            actualHeightOfResult,
                            totalWidth,
                            totalHeight);

                    var message = plainTextEditorPixelReadRequestAction.VirtualizeCoordinateSystemMessage with
                    {
                        VirtualizeCoordinateSystemResult = result
                    };

                    var resultingPlainTextEditor = replacementPlainTextEditor with
                    {
                        RowIndexOffset = startingRowIndex,
                        CharacterColumnIndexOffset = startingCharacterIndex,
                        VirtualizeCoordinateSystemMessage = message,
                        CurrentRowIndex = plainTextEditor.CurrentRowIndex,
                        CurrentTokenIndex = plainTextEditor.CurrentTokenIndex
                    };

                    nextPlainTextEditorMap[plainTextEditorPixelReadRequestAction.PlainTextEditorKey] =
                        resultingPlainTextEditor;

                    var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                    var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                    if (actionRequest.CancellationToken.IsCancellationRequested)
                        return;

                    dispatcher.Dispatch(
                        new SetPlainTextEditorStatesAction(new PlainTextEditorStates(nextImmutableMap,
                            nextImmutableArray)));
                }
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
                    as PlainTextEditorRecordBase;

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

                var resultPlainTextEditorRecord = await PlainTextEditorStates.StateMachine
                    .HandleKeyDownEventAsync(plainTextEditor,
                        overrideKeyDownEventRecord,
                        keyDownEventAction.CancellationToken);

                var replacementPlainTextEditor = resultPlainTextEditorRecord with
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

                var items = replacementPlainTextEditor.Rows
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

                var states = new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);
                
                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(states));
                
                UpdateTokenSemanticDescriptions(states,
                    (PlainTextEditorRecordTokenized) replacementPlainTextEditor,
                    dispatcher);
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
                    as PlainTextEditorRecordBase;

                if (plainTextEditor is null)
                    return;

                var resultPlainTextEditorRecord = await PlainTextEditorStates.StateMachine
                    .HandleOnClickEventAsync(plainTextEditor,
                        plainTextEditorOnClickAction,
                        plainTextEditorOnClickAction.CancellationToken);

                var replacementPlainTextEditor = resultPlainTextEditorRecord with
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

                var items = replacementPlainTextEditor.Rows
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
                    as PlainTextEditorRecordBase;

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

                var fileHandle = await constructMemoryMappedFilePlainTextEditorRecordAction.FileSystemProvider
                    .OpenAsync(constructMemoryMappedFilePlainTextEditorRecordAction.AbsoluteFilePath,
                        constructMemoryMappedFilePlainTextEditorRecordAction.CancellationToken);

                var plainTextEditor = new
                    PlainTextEditorRecordMemoryMappedFile(constructMemoryMappedFilePlainTextEditorRecordAction.PlainTextEditorKey)
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
        public async Task HandleConstructTokenizedPlainTextEditorRecordAction(ConstructTokenizedPlainTextEditorRecordAction constructTokenizedPlainTextEditorRecordAction,
            IDispatcher dispatcher)
        {
            await QueueHandleEffectAsync(async () =>
            {
                var previousPlainTextEditorStates = _plainTextEditorStatesWrap.Value;

                var anEditorIsAlreadyOpenedForTheFile = previousPlainTextEditorStates.Map.Any(x =>
                    (x.Value.FileHandle?.AbsoluteFilePath.GetAbsoluteFilePathString() ?? string.Empty) ==
                    constructTokenizedPlainTextEditorRecordAction.AbsoluteFilePath.GetAbsoluteFilePathString());

                if (anEditorIsAlreadyOpenedForTheFile)
                    return;

                var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
                var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

                var fileHandle = await constructTokenizedPlainTextEditorRecordAction.FileSystemProvider
                    .OpenAsync(constructTokenizedPlainTextEditorRecordAction.AbsoluteFilePath,
                        constructTokenizedPlainTextEditorRecordAction.CancellationToken);

                var plainTextEditor = new
                    PlainTextEditorRecordTokenized(constructTokenizedPlainTextEditorRecordAction.PlainTextEditorKey)
                    {
                        FileHandle = fileHandle
                    };

                nextPlainTextEditorMap[constructTokenizedPlainTextEditorRecordAction.PlainTextEditorKey] = plainTextEditor;
                nextPlainTextEditorList.Add(constructTokenizedPlainTextEditorRecordAction.PlainTextEditorKey);

                var readRequest = new FileHandleReadRequest(
                    0,
                    0,
                    Int32.MaxValue,
                    Int32.MaxValue,
                    constructTokenizedPlainTextEditorRecordAction.CancellationToken);

                var contentRows = await plainTextEditor.FileHandle
                    .ReadAsync(readRequest);

                if (contentRows is null)
                    return;

                var replacementPlainTextEditor = plainTextEditor with
                {
                    SequenceKey = SequenceKey.NewSequenceKey(),
                    FileHandleReadRequest = readRequest,
                    Rows = new IPlainTextEditorRow[]
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

                        var keyDownRecord = new KeyDownEventRecord(
                                character.ToString(),
                                code,
                                false,
                                false,
                                false,
                                IsForced: true
                            );

                        var resultPlainTextEditorRecord = await PlainTextEditorStates.StateMachine
                            .HandleKeyDownEventAsync(replacementPlainTextEditor,
                                keyDownRecord,
                                constructTokenizedPlainTextEditorRecordAction.CancellationToken);

                        replacementPlainTextEditor = ((PlainTextEditorRecordTokenized)resultPlainTextEditorRecord) with
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

                        var newLinedPlainTextEditorRecord = await PlainTextEditorStates.StateMachine
                            .HandleKeyDownEventAsync(replacementPlainTextEditor,
                                forceNewLine,
                                constructTokenizedPlainTextEditorRecordAction.CancellationToken);

                        replacementPlainTextEditor = ((PlainTextEditorRecordTokenized)newLinedPlainTextEditorRecord) with
                        {
                            SequenceKey = SequenceKey.NewSequenceKey()
                        };
                    }
                }

                if (!seenEnterKey)
                    allEnterKeysAreCarriageReturnNewLine = false;
                
                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    RowIndexOffset = readRequest.RowIndexOffset,
                    UseCarriageReturnNewLine = allEnterKeysAreCarriageReturnNewLine
                };

                // TODO: The font-size style attribute does not equal the size of the div that encapsulates the singular character. Figure out EXACTLY these values based off the font-size instead of hard coding what developer tools says
                var heightOfEachRowInPixels = replacementPlainTextEditor.RichTextEditorOptions.HeightOfARowInPixels;
                var widthOfEachCharacterInPixels = replacementPlainTextEditor.RichTextEditorOptions.WidthOfACharacterInPixels;

                var actualWidthOfResult = widthOfEachCharacterInPixels * 
                                          replacementPlainTextEditor.FileHandle.VirtualCharacterLengthOfLongestRow;

                var actualHeightOfResult = heightOfEachRowInPixels * 
                                           replacementPlainTextEditor.FileHandle.VirtualRowCount;

                var totalWidth = widthOfEachCharacterInPixels *
                                 replacementPlainTextEditor.FileHandle.VirtualCharacterLengthOfLongestRow;

                var totalHeight = heightOfEachRowInPixels *
                                  replacementPlainTextEditor.FileHandle.VirtualRowCount;

                var forceCtrlHomeKeyDown = new KeyDownEventRecord(KeyboardKeyFacts.MovementKeys.HOME_KEY,
                    KeyboardKeyFacts.MovementKeys.HOME_KEY,
                    true,
                    false,
                    false,
                    true);

                var resultingPlainTextEditor = (PlainTextEditorRecordTokenized)(await StateMachine
                    .HandleKeyDownEventAsync(replacementPlainTextEditor, forceCtrlHomeKeyDown, CancellationToken.None));

                var items = replacementPlainTextEditor.Rows
                    .Select((row, index) => (index, row));

                var result = new VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(
                    items,
                    items.Select(x => (object)x),
                    actualWidthOfResult,
                    actualHeightOfResult,
                    totalWidth,
                    totalHeight);

                resultingPlainTextEditor = resultingPlainTextEditor with
                {
                    VirtualizeCoordinateSystemMessage = new(
                        new(0, 0, 0, 0, 100, 100, CancellationToken.None),
                        result)
                };

                nextPlainTextEditorMap[constructTokenizedPlainTextEditorRecordAction.PlainTextEditorKey] =
                    resultingPlainTextEditor;

                var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
                var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

                var nextPlainTextEditorStates = new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);

                dispatcher.Dispatch(new SetPlainTextEditorStatesAction(nextPlainTextEditorStates));

                var tokenSemantics = resultingPlainTextEditor.Rows
                    .SelectMany(x => x.Tokens)
                    .Select(x => (x.Key, new SemanticDescription()
                    {
                        SyntaxKind = default,
                        SequenceKey = SequenceKey.NewSequenceKey()
                    }))
                    .ToList();
                
                dispatcher.Dispatch(new UpdateTokenSemanticDescriptionsAction(tokenSemantics));
                
                UpdateTokenSemanticDescriptions(previousPlainTextEditorStates,
                    resultingPlainTextEditor,
                    dispatcher);
                
                if (constructTokenizedPlainTextEditorRecordAction.AbsoluteFilePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.HTML ||
                    constructTokenizedPlainTextEditorRecordAction.AbsoluteFilePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.RAZOR_MARKUP)
                {
                    await ConstructTokenizedHtmlTextEditorRecordAction(nextPlainTextEditorStates,
                        resultingPlainTextEditor,
                        dispatcher);
                }
            });
        }
        
        private async Task ConstructTokenizedHtmlTextEditorRecordAction(PlainTextEditorStates previousPlainTextEditorStates,
            PlainTextEditorRecordTokenized previousEditor,
            IDispatcher dispatcher)
        {
            var startingHtmlTags = new List<string>();
            var endingHtmlTags = new List<string>();

            foreach (var row in previousEditor.Rows)
            {
                foreach (var token in row.Tokens)
                {
                    if (token.Kind == TextTokenKind.Default)
                    {
                        if (token.PlainText.Contains("</"))
                        {
                            endingHtmlTags.Add(token.PlainText);
                        }
                        else if (token.PlainText.Contains("<"))
                        {
                            startingHtmlTags.Add(token.PlainText);
                        }
                    }
                }
            }
        }
        
        private void UpdateTokenSemanticDescriptions(PlainTextEditorStates previousPlainTextEditorStates,
            PlainTextEditorRecordTokenized editor,
            IDispatcher dispatcher)
        {
            if (_updateTokenSemanticDescriptions is not null &&
                _updateTokenSemanticDescriptions.Status == TaskStatus.Running)
            {
                Console.WriteLine("_updateTokenSemanticDescriptions.Status == TaskStatus.Running");
                return;
            }
            
            Console.WriteLine("EnqueueTaskModelAsync _updateTokenSemanticDescriptions");

            _updateTokenSemanticDescriptions = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
                {      
                    var absoluteFilePathValue = new AbsoluteFilePathStringValue(editor.FileHandle.AbsoluteFilePath);

                    if (_solutionStateWrap.Value.FileDocumentMap.TryGetValue(absoluteFilePathValue, out var indexedDocument))
                    {
                        var document = indexedDocument.Document.WithText(SourceText.From(editor.GetPlainText(),
                            editor.FileHandle.Encoding));
                        
                        var syntaxRoot = await document.GetSyntaxRootAsync();

                        indexedDocument.GeneralSyntaxCollector = new();
                        
                        indexedDocument.GeneralSyntaxCollector.Visit(syntaxRoot);

                        var runningTotalOfCharactersInDocument = 0;

                        List<(TextTokenKey textTokenKey, SemanticDescription semanticDescription)> tuples = new();
                        
                        foreach (var row in editor.Rows)
                        {
                            foreach (var token in row.Tokens)
                            {
                                foreach (var propertyDeclaration in indexedDocument.GeneralSyntaxCollector.PropertyDeclarations)
                                {
                                    if (propertyDeclaration.Type.Span.IntersectsWith(new TextSpan(runningTotalOfCharactersInDocument,
                                            token.PlainText.Length)))
                                    {
                                        tuples.Add((token.Key,
                                            new SemanticDescription
                                            {
                                                SequenceKey = SequenceKey.NewSequenceKey(),
                                                SyntaxKind = propertyDeclaration.Kind(),
                                                CssClassString = "pte_plain-text-editor-text-token-display-type"
                                            }));
                                        break;
                                    }
                                }
                                
                                foreach (var methodDeclaration in indexedDocument.GeneralSyntaxCollector.MethodDeclarations)
                                {
                                    if (methodDeclaration.Identifier.Span.IntersectsWith(new TextSpan(runningTotalOfCharactersInDocument,
                                            token.PlainText.Length)))
                                    {
                                        tuples.Add((token.Key,
                                            new SemanticDescription
                                            {
                                                SequenceKey = SequenceKey.NewSequenceKey(),
                                                SyntaxKind = methodDeclaration.Kind(),
                                                CssClassString = "pte_plain-text-editor-text-token-display-method-declaration"
                                            }));
                                        break;
                                    }
                                }
                                
                                runningTotalOfCharactersInDocument += token.PlainText.Length;
                            }
                        }

                        dispatcher.Dispatch(new UpdateTokenSemanticDescriptionsAction(tuples));
                    }
                },
                $"{nameof(PropertyDeclarationCollector)}",
                false,
                TimeSpan.FromSeconds(60));
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
                    as PlainTextEditorRecordBase;

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

