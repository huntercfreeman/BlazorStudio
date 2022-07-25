using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorChunk(PlainTextEditorRowKey Key,
        FileCoordinateGridRequest FileCoordinateGridRequest,
        List<string> Content,
        PlainTextEditorRecord PlainTextEditorRecord)
    {
        public bool OverlapsRequest(FileCoordinateGridRequest currentRequest,
            out PlainTextEditorRecord plainTextEditorRecord)
        {
            // Chunk variables
            var chunkInclusiveStartingRowIndex = FileCoordinateGridRequest.StartingRowIndex;
            var chunkInclusiveStartingCharacterIndex = FileCoordinateGridRequest.StartingCharacterIndex;

            var chunkExclusiveEndingRowIndex = FileCoordinateGridRequest.StartingRowIndex +
                                               FileCoordinateGridRequest.RowCount;
            var chunkExclusiveEndingCharacterIndex = FileCoordinateGridRequest.StartingCharacterIndex +
                                                     FileCoordinateGridRequest.CharacterCount;

            // Current Request variables
            var currentRequestInclusiveStartingRowIndex = currentRequest.StartingRowIndex;
            var currentRequestInclusiveStartingCharacterIndex = currentRequest.StartingCharacterIndex;

            var currentRequestExclusiveEndingRowIndex = currentRequest.StartingRowIndex +
                                               currentRequest.RowCount;
            var currentRequestExclusiveEndingCharacterIndex = currentRequest.StartingCharacterIndex +
                                                     currentRequest.CharacterCount;

            if (chunkInclusiveStartingRowIndex < currentRequestInclusiveStartingRowIndex ||
                    (chunkInclusiveStartingRowIndex == currentRequestInclusiveStartingRowIndex
                        && chunkInclusiveStartingCharacterIndex < currentRequestInclusiveStartingCharacterIndex))
            {
                // If the chunk has content that comes BEFORE the currentRequest

                if (chunkExclusiveEndingRowIndex <= currentRequestExclusiveEndingRowIndex)
                {
                    // If the chunk has content that OVERLAPS the currentRequest

                    var fileCoordinateGridRequest = new FileCoordinateGridRequest(chunkExclusiveEndingRowIndex,
                        currentRequestExclusiveEndingRowIndex - chunkExclusiveEndingRowIndex,
                        chunkExclusiveEndingCharacterIndex,
                        currentRequestExclusiveEndingCharacterIndex - chunkExclusiveEndingCharacterIndex,
                        currentRequest.CancellationToken);

                    var content = PlainTextEditorRecord.FileCoordinateGrid.Request(fileCoordinateGridRequest);

                    var lastRowIndex = PlainTextEditorRecord.List.Count - 1;
                    var lastTokenIndex = PlainTextEditorRecord.List[lastRowIndex].List.Count - 1;

                    PlainTextEditorRecord replacementPlainTextEditor = PlainTextEditorRecord with
                    {
                        CurrentRowIndex = lastRowIndex,
                        CurrentTokenIndex = lastTokenIndex,
                        SequenceKey = SequenceKey.NewSequenceKey(),
                    };

                    
                }
                else
                {
                    // chunkExclusiveEndingRowIndex encompasses the smaller request
                    plainTextEditorRecord = PlainTextEditorRecord;
                    return true;
                }
            }
            else if (chunkExclusiveEndingRowIndex > currentRequestExclusiveEndingRowIndex ||
                     (chunkExclusiveEndingRowIndex == currentRequestExclusiveEndingRowIndex
                      && chunkExclusiveEndingCharacterIndex < currentRequestExclusiveEndingCharacterIndex))
            {
                // If the chunk has content that comes AFTER the currentRequest

                if (chunkExclusiveEndingRowIndex <= currentRequestExclusiveEndingRowIndex)
                {
                    // If the chunk has content that OVERLAPS the currentRequest

                    var fileCoordinateGridRequest = new FileCoordinateGridRequest(startingRowIndex,
                        requestRowCount,
                        startingCharacterIndex,
                        requestCharacterCount,
                        request.CancellationToken);
                }
            }
        }

        private PlainTextEditorRecord AlterChunk(PlainTextEditorRecord plainTextEditorRecord,
            List<string> content)
        {
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

            foreach (var row in content)
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

                    var keyDown = new KeyDownEventAction(PlainTextEditorRecord.PlainTextEditorKey,
                        new KeyDownEventRecord(
                            character.ToString(),
                            code,
                            false,
                            false,
                            false
                        )
                    );

                    plainTextEditorRecord = PlainTextEditorStates.StateMachine
                            .HandleKeyDownEvent(plainTextEditorRecord, keyDown.KeyDownEventRecord) with
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

                    plainTextEditorRecord = PlainTextEditorStates.StateMachine
                        .HandleKeyDownEvent(plainTextEditorRecord, forceNewLine) with
                    {
                        SequenceKey = SequenceKey.NewSequenceKey(),
                    };
                }
            }

            if (seenEnterKey && allEnterKeysAreCarriageReturnNewLine)
            {
                plainTextEditorRecord = plainTextEditorRecord with
                {
                    UseCarriageReturnNewLine = true
                };
            }

            return plainTextEditorRecord;
        }
    }
}
