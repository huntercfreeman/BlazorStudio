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
        public static bool OverlapsRequest(FileCoordinateGridRequest activeRequest,
            FileCoordinateGridRequest chunkRequest,
            List<string> chunkContent,
            PlainTextEditorRecord chunkEditor,
            PlainTextEditorChunk chunk,
            out PlainTextEditorChunk outPlainTextEditorChunk)
        {
            var chunkCoordinates = new RectangleCoordinates
            {
                YMin = chunkRequest.StartingRowIndex,
                YMax = chunkRequest.StartingRowIndex + chunkRequest.RowCount - 1,
                XMin = chunkRequest.StartingCharacterIndex,
                XMax = chunkRequest.StartingCharacterIndex + chunkRequest.CharacterCount - 1
            };
            
            var activeRequestCoordinates = new RectangleCoordinates
            {
                YMin = activeRequest.StartingRowIndex,
                YMax = activeRequest.StartingRowIndex + activeRequest.RowCount - 1,
                XMin = activeRequest.StartingCharacterIndex,
                XMax = activeRequest.StartingCharacterIndex + activeRequest.CharacterCount - 1
            };

            // Edge case: check if chunk encompasses the request
            if (GetLapKindTuples(activeRequestCoordinates, chunkCoordinates)
                    .Count == 4)
            {
                outPlainTextEditorChunk = chunk;
                return true;
            }

            // The general case
            var activeRequestLapMakerTuples = GetLapKindTuples(chunkCoordinates, 
                activeRequestCoordinates);

            if (!activeRequestLapMakerTuples.Any())
            {
                // No overlapping
                outPlainTextEditorChunk = null;
                return false;
            }

            outPlainTextEditorChunk = chunk;

            if (activeRequestLapMakerTuples.Contains((LapKind.North, LapKindModifier.Extends)))
            {
                outPlainTextEditorChunk = ExtendChunkNorth(activeRequest,
                    chunkRequest,
                    chunkContent,
                    chunkEditor,
                    chunk,
                    activeRequestLapMakerTuples);
            }

            return true;
        }

        private static PlainTextEditorChunk ExtendChunkNorth(FileCoordinateGridRequest activeRequest,
            FileCoordinateGridRequest chunkRequest,
            List<string> chunkContent,
            PlainTextEditorRecord chunkEditor,
            PlainTextEditorChunk chunk,
            List<(LapKind lapKind, LapKindModifier lapKindModifier)> lapKindTuples)
        {
            // Non if statement required setting
            int yMin = activeRequest.StartingRowIndex;
            int yExclusiveMax = chunkRequest.StartingRowIndex;
            int yExtensionAmount = yExclusiveMax - yMin;

            // Require if statement
            int xMin = chunkRequest.StartingCharacterIndex;
            int xExclusiveMax = chunkRequest.StartingCharacterIndex + chunkRequest.CharacterCount;
            int xExtensionAmount = 0;

            if (lapKindTuples.Contains((LapKind.West, LapKindModifier.Extends)))
            {
                xMin = activeRequest.StartingCharacterIndex;

                xExtensionAmount += chunkRequest.StartingCharacterIndex - activeRequest.StartingCharacterIndex;
            }
            
            if (lapKindTuples.Contains((LapKind.East, LapKindModifier.Extends)))
            {
                xExclusiveMax = activeRequest.StartingCharacterIndex + activeRequest.CharacterCount;

                xExtensionAmount += (chunkRequest.StartingCharacterIndex + chunkRequest.CharacterCount) 
                                    - (activeRequest.StartingCharacterIndex + activeRequest.CharacterCount);
            }

            var northRequest = new FileCoordinateGridRequest(
                yMin,
                yExclusiveMax - yMin,
                xMin,
                xExclusiveMax - xMin,
                activeRequest.CancellationToken);

            var content = chunk.PlainTextEditorRecord.FileCoordinateGrid
                .Request(northRequest);

            var nextEditor = AlterChunk(chunkEditor,
                content,
                northRequest,
                chunk.FileCoordinateGridRequest);

            var combinedRequest = new FileCoordinateGridRequest(
                yMin,
                yExtensionAmount + chunkRequest.RowCount,
                xMin,
                xExtensionAmount + chunkRequest.CharacterCount,
                activeRequest.CancellationToken);

            nextEditor = nextEditor with
            {
                RowIndexOffset = combinedRequest.StartingRowIndex
            };

            return chunk with
            {
                PlainTextEditorRecord = nextEditor,
                FileCoordinateGridRequest = combinedRequest
            };
        }

        private class RectangleCoordinates
        {
            public double XMin { get; set; }
            public double XMax { get; set; }
            public double YMin { get; set; }
            public double YMax { get; set; }
        }

        /// <summary>
        /// From the lappedItem's perspective
        /// where is it being overlapped?
        /// </summary>
        private enum LapKind
        {
            North,
            West,
            East,
            South
        }

        /// <summary>
        /// If lappedItem has MinY == 50 and lapMaker has MinY == 50
        /// then <see cref="LapKind.North"/> will be returned and the modifier
        /// will be <see cref="LapKindModifier.Equal"/> as this will result
        /// in the cache area being unchanged.
        ///
        /// If lappedItem has MinY == 50 and lapMaker has MinY == 25
        /// then <see cref="LapKind.North"/> will be returned and the modifier
        /// will be <see cref="LapKindModifier.Extends"/> as this will result
        /// in the cache area being extended 25 units north.
        /// </summary>
        private enum LapKindModifier
        {
            Extends,
            Equal,
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lappedItem">
        /// In BlazorStudio.Tests\TestData\CacheTests\PartiallyOverlappedChunks\PartiallyOverlappedChunks.png
        /// this is the black square
        ///
        /// (The existing cached chunk)
        /// </param>
        /// <param name="lapMaker">
        /// In BlazorStudio.Tests\TestData\CacheTests\PartiallyOverlappedChunks\PartiallyOverlappedChunks.png
        /// this is the blue square
        ///
        /// (The active request)
        /// </param>
        /// <returns></returns>
        private static List<(LapKind lapKind, LapKindModifier lapKindModifier)> GetLapKindTuples(RectangleCoordinates lappedItem, RectangleCoordinates lapMaker)
        {
            // Code is ordered N-E-S-W (a compass)

            List<(LapKind lapKind, LapKindModifier lapKindModifier)> lapKinds = new();

            if (lapMaker.YMin < lappedItem.YMin &&
                lapMaker.YMax > lappedItem.YMin)
            {
                lapKinds.Add((LapKind.North, LapKindModifier.Extends));
            }
            else if ((int)lapMaker.YMin == (int)lappedItem.YMin)
            {
                lapKinds.Add((LapKind.North, LapKindModifier.Equal));
            }

            if (lapMaker.XMax > lappedItem.XMax &&
                lapMaker.XMin < lappedItem.XMax)
            {
                lapKinds.Add((LapKind.East, LapKindModifier.Extends));
            }
            else if ((int)lapMaker.XMax == (int)lappedItem.XMax)
            {
                lapKinds.Add((LapKind.East, LapKindModifier.Equal));
            }

            if (lapMaker.YMax > lappedItem.YMax &&
                lapMaker.YMin < lappedItem.YMax)
            {
                lapKinds.Add((LapKind.South, LapKindModifier.Extends));
            }
            else if ((int)lapMaker.YMax == (int)lappedItem.YMax)
            {
                lapKinds.Add((LapKind.South, LapKindModifier.Equal));
            }

            if (lapMaker.XMin < lappedItem.XMin &&
                lapMaker.XMax > lappedItem.XMin)
            {
                lapKinds.Add((LapKind.West, LapKindModifier.Extends));
            }
            else if ((int)lapMaker.XMin == (int)lappedItem.XMin)
            {
                lapKinds.Add((LapKind.West, LapKindModifier.Equal));
            }

            return lapKinds;
        }

        public static PlainTextEditorRecord AlterChunk(PlainTextEditorRecord plainTextEditorRecord,
            List<string> content,
            FileCoordinateGridRequest extensionRequest, 
            FileCoordinateGridRequest originalChunkRequest)
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

            int availableRowCount = extensionRequest.RowCount;

            if (content.Count < extensionRequest.RowCount)
            {
                availableRowCount = content.Count;
            }

            int i = extensionRequest.StartingRowIndex;

            var rowIndex = i + extensionRequest.StartingRowIndex;
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

                // Move to 'CurrentRowIndex = 0' and 'CurrentTokenIndex = 0'
                plainTextEditorRecord = StateMachine.HandleHome(plainTextEditorRecord,
                    new KeyDownEventRecord(
                        KeyboardKeyFacts.MovementKeys.HOME_KEY,
                        KeyboardKeyFacts.MovementKeys.HOME_KEY,
                        true,
                        false,
                        false));
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
                    CurrentRowIndex = correspondingChunkRowIndex,
                    CurrentTokenIndex = 0
                };

                if (extensionRequest.StartingCharacterIndex < originalChunkRequest.StartingCharacterIndex)
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

            for (var index = 0; index < content.Count; index++)
            {
                var line = content[index];

                foreach (var character in line)
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

                if (index == content.Count - 1)
                    continue;

                if (correspondingChunkRowIndex < 0)
                {
                    // Move down one row and insert newline
                    // afterwards write text on the newly inserted empty line

                    plainTextEditorRecord = StateMachine.HandleArrowDown(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                            KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                            false,
                            false,
                            false));

                    plainTextEditorRecord = StateMachine.HandleHome(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.HOME_KEY,
                            KeyboardKeyFacts.MovementKeys.HOME_KEY,
                            false,
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

                    plainTextEditorRecord = StateMachine.HandleArrowUp(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY,
                            KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY,
                            false,
                            false,
                            false));

                    plainTextEditorRecord = StateMachine.HandleHome(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.HOME_KEY,
                            KeyboardKeyFacts.MovementKeys.HOME_KEY,
                            false,
                            false,
                            false));
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
                        CurrentRowIndex = correspondingChunkRowIndex,
                        CurrentTokenIndex = 0
                    };

                    if (extensionRequest.StartingCharacterIndex < originalChunkRequest.StartingCharacterIndex)
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
