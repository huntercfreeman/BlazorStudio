using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
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
                (x.Value.FileCoordinateGrid?.AbsoluteFilePath.GetAbsoluteFilePathString() ?? string.Empty) ==
                    constructMemoryMappedFilePlainTextEditorRecordAction.AbsoluteFilePath.GetAbsoluteFilePathString());

            if (anEditorIsAlreadyOpenedForTheFile)
                return previousPlainTextEditorStates;

            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var fileCoordinateGrid = FileCoordinateGridFactory
                .ConstructFileCoordinateGrid(constructMemoryMappedFilePlainTextEditorRecordAction.AbsoluteFilePath);

            var plainTextEditor = new
                PlainTextEditorRecord(constructMemoryMappedFilePlainTextEditorRecordAction.PlainTextEditorKey)
            {
                FileCoordinateGrid = fileCoordinateGrid
            };

            nextPlainTextEditorMap[constructMemoryMappedFilePlainTextEditorRecordAction.PlainTextEditorKey] = plainTextEditor;
            nextPlainTextEditorList.Add(constructMemoryMappedFilePlainTextEditorRecordAction.PlainTextEditorKey);

            var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
            var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

            return new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);
        }

        [ReducerMethod]
        public PlainTextEditorStates ReduceMemoryMappedFileReadRequestAction(PlainTextEditorStates previousPlainTextEditorStates,
            MemoryMappedFileReadRequestAction memoryMappedFileReadRequestAction)
        {
            var request = memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemMessage
                .VirtualizeCoordinateSystemRequest;

            if (request is null ||
                request.CancellationToken.IsCancellationRequested)
            {
                return previousPlainTextEditorStates;
            }

            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates
                    .Map[memoryMappedFileReadRequestAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            if (plainTextEditor?.FileCoordinateGrid is null)
                return previousPlainTextEditorStates;

            // TODO: The font-size style attribute does not equal the size of the div that encapsulates the singular character. Figure out EXACTLY these values based off the font-size instead of hard coding what developer tools says
            var heightOfEachRowInPixels = 27;
            var widthOfEachCharacterInPixels = 9.91;

            var startingRowIndex =
                (int)(request.ScrollTopInPixels / heightOfEachRowInPixels);

            var requestRowCount = (int)(request.ViewportHeightInPixels / heightOfEachRowInPixels);

            var startingCharacterIndex = (int)(request.ScrollLeftInPixels / widthOfEachCharacterInPixels);

            var requestCharacterCount = (int)(request.ViewportWidthInPixels / widthOfEachCharacterInPixels);

            var fileCoordinateGridRequest = new FileCoordinateGridRequest(startingRowIndex,
                requestRowCount,
                startingCharacterIndex,
                requestCharacterCount,
                request.CancellationToken);

            var chunk = plainTextEditor.UpdateCache(fileCoordinateGridRequest);

            var items = chunk.PlainTextEditorRecord.List
                .Select((row, index) => (index, row))
                .ToList();

            var actualWidthOfResult = widthOfEachCharacterInPixels * requestCharacterCount;

            var actualHeightOfResult = heightOfEachRowInPixels * requestRowCount;

            var totalWidth = widthOfEachCharacterInPixels *
                             chunk.PlainTextEditorRecord.FileCoordinateGrid.CharacterLengthOfLongestRow;

            var totalHeight = heightOfEachRowInPixels *
                              chunk.PlainTextEditorRecord.FileCoordinateGrid.RowCount;

            var result = new VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(
                items,
                items.Select(x => (object)x),
                actualWidthOfResult,
                actualHeightOfResult,
                totalWidth,
                totalHeight);

            var message = memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemMessage with
            {
                VirtualizeCoordinateSystemResult = result
            };

            var resultingPlainTextEditor = chunk.PlainTextEditorRecord with
            {
                LongestRowCharacterLength = (int) chunk.PlainTextEditorRecord.FileCoordinateGrid.CharacterLengthOfLongestRow,
                RowIndexOffset = startingRowIndex,
                VirtualizeCoordinateSystemMessage = message
            };

            nextPlainTextEditorMap[memoryMappedFileReadRequestAction.PlainTextEditorKey] = resultingPlainTextEditor;

            var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
            var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

            if (request.CancellationToken.IsCancellationRequested)
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

            if (plainTextEditor?.FileCoordinateGrid is not null)
                plainTextEditor.FileCoordinateGrid.Dispose();

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

            replacementPlainTextEditor.SetCache(replacementPlainTextEditor);

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

            replacementPlainTextEditor.SetCache(replacementPlainTextEditor);

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

