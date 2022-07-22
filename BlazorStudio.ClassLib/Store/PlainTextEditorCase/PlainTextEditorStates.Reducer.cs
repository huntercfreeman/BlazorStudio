using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
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
            var nextPlainTextEditorMap = new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates
                    .Map[memoryMappedFileReadRequestAction.PlainTextEditorKey]
                as PlainTextEditorRecord;

            if (plainTextEditor is null)
                return previousPlainTextEditorStates;

            var heightOfEachRowInPixels = 2 * plainTextEditor.RichTextEditorOptions.FontSizeInPixels;

            var startingRowIndex = 
                (int)(memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ScrollTop / heightOfEachRowInPixels);

            var rowCount = 
                (int)(memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.ViewportHeight / heightOfEachRowInPixels);

            var fileCoordinateGridRequest = new FileCoordinateGridRequest(startingRowIndex, 
                rowCount,
                memoryMappedFileReadRequestAction.VirtualizeCoordinateSystemRequest.CancellationToken);

            var content = plainTextEditor.FileCoordinateGrid!
                .Request(fileCoordinateGridRequest);

            var allEnterKeysAreCarriageReturnNewLine = true;
            var seenEnterKey = false;
            var previousCharacterWasCarriageReturn = false;

            string MutateIfPreviousCharacterWasCarriageReturn()
            {
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
                    .Add(plainTextEditor.GetEmptyPlainTextEditorRow())
            };

            foreach (var character in content)
            {
                if (character == '\r')
                {
                    previousCharacterWasCarriageReturn = true;
                    continue;
                }

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

            if (seenEnterKey && allEnterKeysAreCarriageReturnNewLine)
            {
                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    UseCarriageReturnNewLine = true
                };
            }

            nextPlainTextEditorMap[memoryMappedFileReadRequestAction.PlainTextEditorKey] = replacementPlainTextEditor;

            var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
            var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

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
         
            return new PlainTextEditorStates(nextPlainTextEditorMap.ToImmutableDictionary(), 
                nextPlainTextEditorList.ToImmutableArray());
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

