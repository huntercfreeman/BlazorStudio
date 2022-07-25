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
                    // chunkExclusiveEndingRowIndex encompasses the smaller request
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
                    // chunkExclusiveEndingRowIndex encompasses the smaller request
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

            if (currentRequestInclusiveStartingRowIndex < chunkInclusiveStartingRowIndex)
            {
                var subrequestInclusiveStartingRowIndex = currentRequestInclusiveStartingRowIndex;

                var subrequestRowCount = chunkInclusiveStartingRowIndex - currentRequestInclusiveStartingRowIndex;

                var subrequestStartingCharacterIndex = Math.Min(currentRequestInclusiveStartingCharacterIndex,
                    chunkInclusiveStartingCharacterIndex);

                var subrequestCharacterCount = Math.Max(currentRequestExclusiveEndingCharacterIndex,
                    chunkExclusiveEndingCharacterIndex)
                        - subrequestStartingCharacterIndex;

                var subrequest = new FileCoordinateGridRequest(
                    subrequestInclusiveStartingRowIndex,
                    subrequestRowCount,
                    subrequestStartingCharacterIndex,
                    subrequestCharacterCount,
                    currentRequest.CancellationToken);

                var content = PlainTextEditorRecord.FileCoordinateGrid.Request(subrequest);

                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    CurrentRowIndex = 0,
                    CurrentTokenIndex = 0,
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                replacementPlainTextEditor = AlterChunk(replacementPlainTextEditor,
                    content);
            }
            
            if (currentRequestExclusiveEndingRowIndex > chunkExclusiveEndingRowIndex)
            {
                var subrequestInclusiveStartingRowIndex = chunkExclusiveEndingRowIndex;

                var subrequestRowCount = currentRequestExclusiveEndingRowIndex - chunkExclusiveEndingRowIndex;

                var subrequestStartingCharacterIndex = Math.Min(currentRequestInclusiveStartingCharacterIndex,
                    chunkInclusiveStartingCharacterIndex);

                var subrequestCharacterCount = Math.Max(currentRequestExclusiveEndingCharacterIndex,
                    chunkExclusiveEndingCharacterIndex)
                        - subrequestStartingCharacterIndex;

                var subrequest = new FileCoordinateGridRequest(
                    subrequestInclusiveStartingRowIndex,
                    subrequestRowCount,
                    subrequestStartingCharacterIndex,
                    subrequestCharacterCount,
                    currentRequest.CancellationToken);

                var content = PlainTextEditorRecord.FileCoordinateGrid.Request(subrequest);

                replacementPlainTextEditor = replacementPlainTextEditor with
                {
                    CurrentRowIndex = 0,
                    CurrentTokenIndex = 0,
                    SequenceKey = SequenceKey.NewSequenceKey()
                };

                replacementPlainTextEditor = AlterChunk(replacementPlainTextEditor,
                    content);
            }
        }

        public static PlainTextEditorRecord AlterChunk(PlainTextEditorRecord plainTextEditorRecord,
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
