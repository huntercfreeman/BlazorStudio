using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Virtualize;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    public class PlainTextEditorStatesReducer
    {
        [ReducerMethod]
        public static PlainTextEditorStates ReduceConstructInMemoryPlainTextEditorRecordAction(PlainTextEditorStates previousPlainTextEditorStates,
            ConstructInMemoryPlainTextEditorRecordAction constructInMemoryPlainTextEditorRecordAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = new
                PlainTextEditorRecord(constructInMemoryPlainTextEditorRecordAction.PlainTextEditorKey);

            nextPlainTextEditorMap[constructInMemoryPlainTextEditorRecordAction.PlainTextEditorKey] = plainTextEditor;
            nextPlainTextEditorList.Add(constructInMemoryPlainTextEditorRecordAction.PlainTextEditorKey);

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }

        [ReducerMethod]
        public PlainTextEditorStates ReduceConstructMemoryMappedFilePlainTextEditorRecordAction(PlainTextEditorStates previousPlainTextEditorStates,
            ConstructMemoryMappedFilePlainTextEditorRecordAction constructMemoryMappedFilePlainTextEditorRecordAction)
        {
            var anEditorIsAlreadyOpenedForTheFile = previousPlainTextEditorStates.Map.Any(x =>
                (x.Value.FileHandle?.AbsoluteFilePath.GetAbsoluteFilePathString() ?? string.Empty) ==
                    constructMemoryMappedFilePlainTextEditorRecordAction.AbsoluteFilePath.GetAbsoluteFilePathString());

            if (anEditorIsAlreadyOpenedForTheFile)
                return previousPlainTextEditorStates;

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

            return new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);
        }

        [ReducerMethod]
        public PlainTextEditorStates ReduceMemoryMappedFilePixelReadRequestAction(PlainTextEditorStates previousPlainTextEditorStates,
            MemoryMappedFilePixelReadRequestAction memoryMappedFilePixelReadRequestAction)
        {
            var actionRequest = memoryMappedFilePixelReadRequestAction.VirtualizeCoordinateSystemMessage
                .VirtualizeCoordinateSystemRequest;

            if (actionRequest is null ||
                actionRequest.CancellationToken.IsCancellationRequested)
            {
                return previousPlainTextEditorStates;
            }

            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates
                    .Map[memoryMappedFilePixelReadRequestAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            if (plainTextEditor?.FileHandle is null)
                return previousPlainTextEditorStates;

            // TODO: The font-size style attribute does not equal the size of the div that encapsulates the singular character. Figure out EXACTLY these values based off the font-size instead of hard coding what developer tools says
            var heightOfEachRowInPixels = 27;
            var widthOfEachCharacterInPixels = 9.91;

            var startingRowIndex =
                (int)(actionRequest.ScrollTopInPixels / heightOfEachRowInPixels);

            var requestRowCount = (int)(actionRequest.ViewportHeightInPixels / heightOfEachRowInPixels);

            var startingCharacterIndex = (int)(actionRequest.ScrollLeftInPixels / widthOfEachCharacterInPixels);

            var requestCharacterCount = (int)(actionRequest.ViewportWidthInPixels / widthOfEachCharacterInPixels);

            var fileCoordinateGridRequest = new FileCoordinateGridRequest(startingRowIndex,
                requestRowCount,
                startingCharacterIndex,
                requestCharacterCount,
                actionRequest.CancellationToken);

            var contentRows = plainTextEditor.FileHandle
                .Read(new FileHandleReadRequest(
                    fileCoordinateGridRequest.StartingRowIndex,
                    fileCoordinateGridRequest.StartingCharacterIndex,
                    fileCoordinateGridRequest.RowCount,
                    fileCoordinateGridRequest.CharacterCount,
                    CancellationToken.None));

            var replacementPlainTextEditor = plainTextEditor with
            {
                SequenceKey = SequenceKey.NewSequenceKey(),
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
                RowIndexOffset = fileCoordinateGridRequest.StartingRowIndex
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
                LongestRowCharacterLength = (int)replacementPlainTextEditor.FileHandle.VirtualCharacterLengthOfLongestRow,
                RowIndexOffset = startingRowIndex,
                VirtualizeCoordinateSystemMessage = message
            };

            nextPlainTextEditorMap[memoryMappedFilePixelReadRequestAction.PlainTextEditorKey] = resultingPlainTextEditor;

            var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
            var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

            if (actionRequest.CancellationToken.IsCancellationRequested)
                return previousPlainTextEditorStates;

            return new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);
        }

        [ReducerMethod]
        public static PlainTextEditorStates ReduceDeconstructPlainTextEditorRecordAction(PlainTextEditorStates previousPlainTextEditorStates,
            DeconstructPlainTextEditorRecordAction deconstructPlainTextEditorRecordAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates
                    .Map[deconstructPlainTextEditorRecordAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            nextPlainTextEditorMap.Remove(deconstructPlainTextEditorRecordAction.PlainTextEditorKey);
            nextPlainTextEditorList.Remove(deconstructPlainTextEditorRecordAction.PlainTextEditorKey);

            if (plainTextEditor?.FileHandle is not null)
                plainTextEditor.FileHandle.Dispose();

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }

        [ReducerMethod]
        public PlainTextEditorStates ReduceKeyDownEventAction(PlainTextEditorStates previousPlainTextEditorStates,
            KeyDownEventAction keyDownEventAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates.Map[keyDownEventAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            if (plainTextEditor is null)
                return previousPlainTextEditorStates;

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

            return new PlainTextEditorStates(nextImmutableMap,
                nextImmutableArray);
        }

        [ReducerMethod]
        public static PlainTextEditorStates ReducePlainTextEditorOnClickAction(PlainTextEditorStates previousPlainTextEditorStates,
            PlainTextEditorOnClickAction plainTextEditorOnClickAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates.Map[plainTextEditorOnClickAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            if (plainTextEditor is null)
                return previousPlainTextEditorStates;

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

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }
        
        [ReducerMethod]
        public static PlainTextEditorStates ReduceSetIsReadonlyAction(PlainTextEditorStates previousPlainTextEditorStates,
            SetIsReadonlyAction setIsReadonlyAction)
        {
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates.Map[setIsReadonlyAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            if (plainTextEditor is null)
                return previousPlainTextEditorStates;

            var replacementPlainTextEditor = plainTextEditor with
            {
                SequenceKey = SequenceKey.NewSequenceKey(),
                IsReadonly = setIsReadonlyAction.IsReadonly
            };

            nextPlainTextEditorMap[setIsReadonlyAction.PlainTextEditorKey] = replacementPlainTextEditor;

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }
    }
}

