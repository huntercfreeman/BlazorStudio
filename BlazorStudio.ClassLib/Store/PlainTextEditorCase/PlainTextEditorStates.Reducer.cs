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
            if (memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.CancellationToken
                .IsCancellationRequested)
            {
                return previousPlainTextEditorStates;
            }

            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates
                    .Map[memoryMappedFileReadRequestAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            if (plainTextEditor is null)
                return previousPlainTextEditorStates;

            // TODO: The font-size style attribute does not equal the size of the div that encapsulates the singular character. Figure out EXACTLY these values based off the font-size instead of hard coding what developer tools says
            var heightOfEachRowInPixels = 27;
            var widthOfEachCharacterInPixels = 9.91;

            var paddingInPixels = 250;

            var scrollTopWithPadding = memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ScrollTop -
                                       paddingInPixels;

            scrollTopWithPadding = scrollTopWithPadding < 0
                ? 0
                : scrollTopWithPadding;

            var startingRowIndex = 
                (int)(scrollTopWithPadding / heightOfEachRowInPixels);

            var viewportHeightWithPadding = memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ViewportHeight
                + 250;

            viewportHeightWithPadding = viewportHeightWithPadding >
                                        memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest
                                            .ScrollHeight
                                        ? memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest
                                            .ScrollHeight
                                        : viewportHeightWithPadding;

            var rowCount = 
                (int)(viewportHeightWithPadding / heightOfEachRowInPixels);

            var scrollLeftWithPadding = memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ScrollLeft
                - 250;

            scrollLeftWithPadding = scrollLeftWithPadding < 0
                ? 0
                : scrollLeftWithPadding;

            var startingCharacterIndex =
                (int)(scrollLeftWithPadding / widthOfEachCharacterInPixels);

            var viewportWidthWithPadding =
                memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ViewportWidth + 250;

            viewportWidthWithPadding = viewportWidthWithPadding >
                                       memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ScrollWidth
                ? memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ScrollWidth
                : viewportWidthWithPadding;

            var characterCount =
                (int)(viewportWidthWithPadding / widthOfEachCharacterInPixels);

            var fileCoordinateGridRequest = new FileCoordinateGridRequest(startingRowIndex, 
                rowCount,
                startingCharacterIndex,
                characterCount,
                memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.CancellationToken);

            var contentOfRows = plainTextEditor.FileCoordinateGrid!
                .Request(fileCoordinateGridRequest);

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

            PlainTextEditorRecord replacementPlainTextEditor = plainTextEditor with
            {
                CurrentRowIndex = 0,
                CurrentTokenIndex = 0,
                SequenceKey = SequenceKey.NewSequenceKey(),
                List = ImmutableList<IPlainTextEditorRow>.Empty
                    .Add(plainTextEditor.GetEmptyPlainTextEditorRow()),
            };

            foreach (var row in contentOfRows)
            {
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

                    var keyDown = new KeyDownEventAction(plainTextEditor.PlainTextEditorKey,
                        new KeyDownEventRecord(
                            character.ToString(),
                            code,
                            false,
                            false,
                            false
                        )
                    );

                    replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                            .HandleKeyDownEvent(replacementPlainTextEditor, keyDown.KeyDownEventRecord) with
                        {
                            SequenceKey = SequenceKey.NewSequenceKey()
                        };

                    previousCharacterWasCarriageReturn = false;
                }

                if (row.LastOrDefault() != '\n')
                {
                    var forceNewLine = new KeyDownEventRecord(
                        KeyboardKeyFacts.NewLineCodes.ENTER_CODE,
                        KeyboardKeyFacts.NewLineCodes.ENTER_CODE,
                        false,
                        false,
                        false);

                    replacementPlainTextEditor = PlainTextEditorStates.StateMachine
                        .HandleKeyDownEvent(replacementPlainTextEditor, forceNewLine) with
                        {
                            SequenceKey = SequenceKey.NewSequenceKey(),
                            
                        };
                }
            }

            if (seenEnterKey && allEnterKeysAreCarriageReturnNewLine)
            {
                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    UseCarriageReturnNewLine = true
                };
            }

            replacementPlainTextEditor = replacementPlainTextEditor with
            {
                LongestRowCharacterLength = longestRowCharacterLength,
                RowIndexOffset = startingRowIndex,
                VirtualizeCoordinateSystemResult = new VirtualizeCoordinateSystemResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(
                    memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest,
                    replacementPlainTextEditor.List
                        .Select((row, index) => (index, row)),
                    widthOfEachCharacterInPixels * replacementPlainTextEditor.FileCoordinateGrid.CharacterLengthOfLongestRow,
                    heightOfEachRowInPixels * replacementPlainTextEditor.FileCoordinateGrid.CharacterLengthOfLongestRow),
            };

            nextPlainTextEditorMap[memoryMappedFileReadRequestAction.PlainTextEditorKey] = replacementPlainTextEditor;

            var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
            var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

            if (memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.CancellationToken
                .IsCancellationRequested)
            {
                return previousPlainTextEditorStates;
            }

            return new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);
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

            nextPlainTextEditorMap[plainTextEditorOnClickAction.PlainTextEditorKey] = replacementPlainTextEditor;

            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), nextPlainTextEditorList.ToImmutableArray());
        }
    }
}

