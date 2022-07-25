using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorChunk(FileCoordinateGridRequest FileCoordinateGridRequest,
        List<string> Content,
        PlainTextEditorRecord PlainTextEditorRecord)
    {
        public bool OverlapsRequest(FileCoordinateGridRequest currentRequest,
            out PlainTextEditorChunk outPlainTextEditorChunk)
        {
            // Chunk coordinate variables
            var chunkInclusiveStartingRowIndex = FileCoordinateGridRequest.StartingRowIndex;
            var chunkInclusiveStartingCharacterIndex = FileCoordinateGridRequest.StartingCharacterIndex;

            var chunkExclusiveEndingRowIndex = FileCoordinateGridRequest.StartingRowIndex +
                                               FileCoordinateGridRequest.RowCount;
            var chunkExclusiveEndingCharacterIndex = FileCoordinateGridRequest.StartingCharacterIndex +
                                                     FileCoordinateGridRequest.CharacterCount;

            // Current Request coordinate variables
            var currentRequestInclusiveStartingRowIndex = currentRequest.StartingRowIndex;
            var currentRequestInclusiveStartingCharacterIndex = currentRequest.StartingCharacterIndex;

            var currentRequestExclusiveEndingRowIndex = currentRequest.StartingRowIndex +
                                               currentRequest.RowCount;
            var currentRequestExclusiveEndingCharacterIndex = currentRequest.StartingCharacterIndex +
                                                     currentRequest.CharacterCount;

            bool hasOverlappingContent = false;

            if (chunkInclusiveStartingRowIndex < currentRequestInclusiveStartingRowIndex ||
                    (chunkInclusiveStartingRowIndex == currentRequestInclusiveStartingRowIndex
                        && chunkInclusiveStartingCharacterIndex < currentRequestInclusiveStartingCharacterIndex))
            {
                // If the chunk has content that comes BEFORE the currentRequest
                if (chunkExclusiveEndingRowIndex <= currentRequestExclusiveEndingRowIndex &&
                    currentRequestExclusiveEndingRowIndex > chunkExclusiveEndingRowIndex)
                {
                    // If the chunk has content that OVERLAPS the currentRequest
                    hasOverlappingContent = true;
                }
                else
                {
                    // chunkExclusiveEndingRowIndex ENCOMPASSES the smaller request
                    outPlainTextEditorChunk = this;
                    return true;
                }
            }
            else if (chunkExclusiveEndingRowIndex > currentRequestExclusiveEndingRowIndex ||
                     (chunkExclusiveEndingRowIndex == currentRequestExclusiveEndingRowIndex
                      && chunkExclusiveEndingCharacterIndex < currentRequestExclusiveEndingCharacterIndex))
            {
                // If the chunk has content that comes AFTER the currentRequest
                if (chunkExclusiveEndingRowIndex > currentRequestExclusiveEndingRowIndex &&
                    chunkInclusiveStartingRowIndex > currentRequestInclusiveStartingRowIndex)
                {
                    // If the chunk has content that OVERLAPS the currentRequest

                    hasOverlappingContent = true;
                }
                else
                {
                    // chunkExclusiveEndingRowIndex ENCOMPASSES the smaller request
                    outPlainTextEditorChunk = this;
                    return true;
                }
            }

            if (!hasOverlappingContent)
            {
                outPlainTextEditorChunk = this;
                return false;
            }

            // Has overlapping content so square off the overlap and return it as a new chunk

            PlainTextEditorRecord replacementPlainTextEditor = PlainTextEditorRecord with
            {
                CurrentRowIndex = 0,
                CurrentTokenIndex = 0,
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            FileCoordinateGridRequest subrequest;

            // Square off north end
            if (currentRequestInclusiveStartingRowIndex < chunkInclusiveStartingRowIndex)
            {
                var subrequestInclusiveStartingRowIndex = currentRequestInclusiveStartingRowIndex;

                var subrequestRowCount = chunkInclusiveStartingRowIndex - currentRequestInclusiveStartingRowIndex;

                var subrequestStartingCharacterIndex = Math.Min(currentRequestInclusiveStartingCharacterIndex,
                    chunkInclusiveStartingCharacterIndex);

                var subrequestCharacterCount = Math.Max(currentRequestExclusiveEndingCharacterIndex,
                    chunkExclusiveEndingCharacterIndex)
                        - subrequestStartingCharacterIndex;

                subrequest = new FileCoordinateGridRequest(
                    subrequestInclusiveStartingRowIndex,
                    subrequestRowCount,
                    subrequestStartingCharacterIndex,
                    subrequestCharacterCount,
                    currentRequest.CancellationToken);

                var content = PlainTextEditorRecord.FileCoordinateGrid.Request(subrequest);

                replacementPlainTextEditor = AlterChunk(replacementPlainTextEditor,
                    content,
                    subrequest,
                    FileCoordinateGridRequest);
            }
            else if (chunkInclusiveStartingRowIndex < currentRequestInclusiveStartingRowIndex)
            {
                var subrequestInclusiveStartingRowIndex = chunkInclusiveStartingRowIndex;

                var subrequestRowCount = currentRequestInclusiveStartingRowIndex - chunkInclusiveStartingRowIndex ;

                var subrequestStartingCharacterIndex = Math.Min(currentRequestInclusiveStartingCharacterIndex,
                    chunkInclusiveStartingCharacterIndex);

                var subrequestCharacterCount = Math.Max(currentRequestExclusiveEndingCharacterIndex,
                                                   chunkExclusiveEndingCharacterIndex)
                                               - subrequestStartingCharacterIndex;

                subrequest = new FileCoordinateGridRequest(
                    subrequestInclusiveStartingRowIndex,
                    subrequestRowCount,
                    subrequestStartingCharacterIndex,
                    subrequestCharacterCount,
                    currentRequest.CancellationToken);

                var content = PlainTextEditorRecord.FileCoordinateGrid.Request(subrequest);

                replacementPlainTextEditor = AlterChunk(replacementPlainTextEditor,
                    content,
                    subrequest,
                    FileCoordinateGridRequest);
            }

            // Square off south end
            if (currentRequestExclusiveEndingRowIndex > chunkExclusiveEndingRowIndex)
            {
                var subrequestInclusiveStartingRowIndex = chunkExclusiveEndingRowIndex;

                var subrequestRowCount = currentRequestExclusiveEndingRowIndex - chunkExclusiveEndingRowIndex;

                var subrequestStartingCharacterIndex = Math.Min(currentRequestInclusiveStartingCharacterIndex,
                    chunkInclusiveStartingCharacterIndex);

                var subrequestCharacterCount = Math.Max(currentRequestExclusiveEndingCharacterIndex,
                    chunkExclusiveEndingCharacterIndex)
                        - subrequestStartingCharacterIndex;

                subrequest = new FileCoordinateGridRequest(
                    subrequestInclusiveStartingRowIndex,
                    subrequestRowCount,
                    subrequestStartingCharacterIndex,
                    subrequestCharacterCount,
                    currentRequest.CancellationToken);

                var content = PlainTextEditorRecord.FileCoordinateGrid.Request(subrequest);

                replacementPlainTextEditor = AlterChunk(replacementPlainTextEditor,
                    content,
                    subrequest,
                    FileCoordinateGridRequest);
            }

            // Square off center

            {
                int subrequestInclusiveStartingRowIndex = chunkInclusiveStartingRowIndex;
                int subrequestRowCount = chunkExclusiveEndingRowIndex - chunkInclusiveStartingRowIndex;

                int subrequestStartingCharacterIndex = currentRequestInclusiveStartingCharacterIndex;
                int subrequestCharacterCount = currentRequestExclusiveEndingCharacterIndex -
                                                   Math.Min(currentRequestInclusiveStartingCharacterIndex,
                                                       chunkInclusiveStartingCharacterIndex);

                subrequest = new FileCoordinateGridRequest(
                    subrequestInclusiveStartingRowIndex,
                    subrequestRowCount,
                    subrequestStartingCharacterIndex,
                    subrequestCharacterCount,
                    currentRequest.CancellationToken);

                var content = PlainTextEditorRecord.FileCoordinateGrid.Request(subrequest);

                replacementPlainTextEditor = AlterChunk(replacementPlainTextEditor,
                    content,
                    subrequest,
                    FileCoordinateGridRequest);
            }

            outPlainTextEditorChunk = new PlainTextEditorChunk(
                subrequest,

                // TODO: Track Content
                new(),

                replacementPlainTextEditor);

            return true;
        }

        public static PlainTextEditorRecord AlterChunk(PlainTextEditorRecord plainTextEditorRecord,
            List<string> content,
            FileCoordinateGridRequest subrequest, 
            FileCoordinateGridRequest chunkRequest)
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

            for (int i = subrequest.StartingRowIndex;
                 i < subrequest.RowCount;
                 i++)
            {
                var rowIndex = i + subrequest.StartingRowIndex;
                var correspondingChunkRowIndex = rowIndex - plainTextEditorRecord.RowIndexOffset;

                if (correspondingChunkRowIndex < 0)
                {
                    // Chunk cannot match the row so make a new empty row for insertion
                    // Move to 'CurrentRowIndex = 0' and 'CurrentTokenIndex = 0'
                    plainTextEditorRecord = StateMachine.HandleHome(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.HOME_KEY,
                            KeyboardKeyFacts.MovementKeys.HOME_KEY,
                            true,
                            false,
                            false));

                    string characterCode = plainTextEditorRecord.UseCarriageReturnNewLine 
                        ? KeyboardKeyFacts.NewLineCodes.CARRIAGE_RETURN_NEW_LINE_CODE 
                        : KeyboardKeyFacts.NewLineCodes.ENTER_CODE;

                    // Insert a row above the first row
                    plainTextEditorRecord = StateMachine.HandleKeyDownEvent(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            characterCode,
                            characterCode,
                            false,
                            false,
                            false));

                    // Set position on the previously inserted row at token index 0
                    plainTextEditorRecord = plainTextEditorRecord with
                    {
                        CurrentRowIndex = plainTextEditorRecord.CurrentRowIndex - 1,
                        CurrentTokenIndex = 0
                    };
                }
                else if (correspondingChunkRowIndex > plainTextEditorRecord.List.Count)
                {
                    // Chunk cannot match the row so make a new empty row for insertion
                    // Move to the end of the chunk
                    plainTextEditorRecord = StateMachine.HandleEnd(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.END_KEY,
                            KeyboardKeyFacts.MovementKeys.END_KEY,
                            true,
                            false,
                            false));

                    string characterCode = plainTextEditorRecord.UseCarriageReturnNewLine 
                        ? KeyboardKeyFacts.NewLineCodes.CARRIAGE_RETURN_NEW_LINE_CODE 
                        : KeyboardKeyFacts.NewLineCodes.ENTER_CODE;

                    // Insert a row above the first row
                    plainTextEditorRecord = StateMachine.HandleKeyDownEvent(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            characterCode,
                            characterCode,
                            false,
                            false,
                            false));

                    // Position is on the previously inserted row at token index 0
                }
                else
                {
                    // Chunk CAN match the row
                    // Move to the position

                    plainTextEditorRecord = plainTextEditorRecord with
                    {
                        CurrentRowIndex = correspondingChunkRowIndex
                    };

                    if (subrequest.StartingCharacterIndex < chunkRequest.StartingCharacterIndex)
                    {
                        // First character in corresponding row
                        plainTextEditorRecord = StateMachine.HandleHome(plainTextEditorRecord,
                            new KeyDownEventRecord(
                                KeyboardKeyFacts.MovementKeys.HOME_KEY,
                                KeyboardKeyFacts.MovementKeys.HOME_KEY,
                                false,
                                false,
                                false));
                    }
                    else
                    {
                        // Last character in corresponding row
                        plainTextEditorRecord = StateMachine.HandleEnd(plainTextEditorRecord,
                            new KeyDownEventRecord(
                                KeyboardKeyFacts.MovementKeys.END_KEY,
                                KeyboardKeyFacts.MovementKeys.END_KEY,
                                false,
                                false,
                                false));
                    }
                }

                foreach (var character in content[i])
                {
                    if (character == '\r')
                    {
                        previousCharacterWasCarriageReturn = true;
                        continue;
                    }

                    if (character == '\n')
                    {
                        // TODO: I think ignoring this is correct but unsure
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

                    var keyDown = new KeyDownEventAction(plainTextEditorRecord.PlainTextEditorKey,
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
