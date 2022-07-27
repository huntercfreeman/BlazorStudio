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
            PlainTextEditorChunk chunk,
            out PlainTextEditorChunk outPlainTextEditorChunk)
        {
            var chunkCoordinates = new RectangleCoordinates
            {
                YMin = chunk.FileCoordinateGridRequest.StartingRowIndex,
                YMax = chunk.FileCoordinateGridRequest.StartingRowIndex + chunk.FileCoordinateGridRequest.RowCount - 1,
                XMin = chunk.FileCoordinateGridRequest.StartingCharacterIndex,
                XMax = chunk.FileCoordinateGridRequest.StartingCharacterIndex + chunk.FileCoordinateGridRequest.CharacterCount - 1
            };
            
            var activeRequestCoordinates = new RectangleCoordinates
            {
                YMin = activeRequest.StartingRowIndex,
                YMax = activeRequest.StartingRowIndex + activeRequest.RowCount - 1,
                XMin = activeRequest.StartingCharacterIndex,
                XMax = activeRequest.StartingCharacterIndex + activeRequest.CharacterCount - 1
            };

            // Edge case: check if chunk encompasses the request
            {
                var refToLapKindTuplesForDebugging =
                    GetLapKindTuples(activeRequestCoordinates, chunkCoordinates);

                if (refToLapKindTuplesForDebugging
                        .Count == 4)
                {
                    outPlainTextEditorChunk = chunk;
                    return true;
                }
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

            if (activeRequestLapMakerTuples.Contains((LapKind.North, LapKindModifier.Extends)))
            {
                chunk = ExtendChunkNorth(activeRequest,
                    chunk,
                    activeRequestLapMakerTuples);
            }
            
            if (activeRequestLapMakerTuples.Contains((LapKind.South, LapKindModifier.Extends)))
            {
                chunk = ExtendChunkSouth(activeRequest,
                    chunk,
                    activeRequestLapMakerTuples);
            }
            
            if (activeRequestLapMakerTuples.Contains((LapKind.East, LapKindModifier.Extends)))
            {
                chunk = ExtendChunkEast(activeRequest,
                    chunk,
                    activeRequestLapMakerTuples);
            }

            if (activeRequestLapMakerTuples.Contains((LapKind.West, LapKindModifier.Extends)))
            {
                chunk = ExtendChunkWest(activeRequest,
                    chunk,
                    activeRequestLapMakerTuples);
            }

            outPlainTextEditorChunk = chunk;
            return true;
        }

        private static PlainTextEditorChunk ExtendChunkNorth(FileCoordinateGridRequest activeRequest,
            PlainTextEditorChunk chunk,
            List<(LapKind lapKind, LapKindModifier lapKindModifier)> lapKindTuples)
        {
            // Non if statement required setting
            int yMin = activeRequest.StartingRowIndex;
            int yExclusiveMax = chunk.FileCoordinateGridRequest.StartingRowIndex;
            int yExtensionAmount = yExclusiveMax - yMin;

            // Require if statement
            int xMin = chunk.FileCoordinateGridRequest.StartingCharacterIndex;
            int xExclusiveMax = chunk.FileCoordinateGridRequest.StartingCharacterIndex + chunk.FileCoordinateGridRequest.CharacterCount;
            int xExtensionAmount = 0;

            var westExtension = (LapKind.West, LapKindModifier.Extends);
            if (lapKindTuples.Contains(westExtension))
            {
                xMin = activeRequest.StartingCharacterIndex;

                xExtensionAmount += chunk.FileCoordinateGridRequest.StartingCharacterIndex - activeRequest.StartingCharacterIndex;
            }
            
            var eastExtension = (LapKind.East, LapKindModifier.Extends);
            if (lapKindTuples.Contains(eastExtension))
            {
                xExclusiveMax = activeRequest.StartingCharacterIndex + activeRequest.CharacterCount;

                xExtensionAmount += (chunk.FileCoordinateGridRequest.StartingCharacterIndex + chunk.FileCoordinateGridRequest.CharacterCount) 
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

            var nextEditor = AlterChunk(chunk.PlainTextEditorRecord,
                content,
                northRequest,
                chunk.FileCoordinateGridRequest);

            var combinedRequest = new FileCoordinateGridRequest(
                yMin,
                yExtensionAmount + chunk.FileCoordinateGridRequest.RowCount,
                xMin,
                xExtensionAmount + chunk.FileCoordinateGridRequest.CharacterCount,
                activeRequest.CancellationToken);

            nextEditor = nextEditor with
            {
                RowIndexOffset = combinedRequest.StartingRowIndex
            };

            lapKindTuples.Remove((LapKind.North, LapKindModifier.Extends));

            return chunk with
            {
                PlainTextEditorRecord = nextEditor,
                FileCoordinateGridRequest = combinedRequest
            };
        }

        private static PlainTextEditorChunk ExtendChunkEast(FileCoordinateGridRequest activeRequest,
            PlainTextEditorChunk chunk,
            List<(LapKind lapKind, LapKindModifier lapKindModifier)> lapKindTuples)
        {
            // Non if statement required setting
            int xMin = chunk.FileCoordinateGridRequest.StartingCharacterIndex + chunk.FileCoordinateGridRequest.CharacterCount;
            int xExclusiveMax = activeRequest.StartingCharacterIndex + activeRequest.CharacterCount;
            int xExtensionAmount = xExclusiveMax - xMin;

            int yMin = chunk.FileCoordinateGridRequest.StartingRowIndex;
            int yExclusiveMax = chunk.FileCoordinateGridRequest.StartingRowIndex + chunk.FileCoordinateGridRequest.RowCount;

            var eastRequest = new FileCoordinateGridRequest(
                yMin,
                yExclusiveMax - yMin,
                xMin,
                xExclusiveMax - xMin,
                activeRequest.CancellationToken);

            var content = chunk.PlainTextEditorRecord.FileCoordinateGrid
                .Request(eastRequest);

            var nextEditor = AlterChunk(chunk.PlainTextEditorRecord,
                content,
                eastRequest,
                chunk.FileCoordinateGridRequest);

            var combinedRequest = new FileCoordinateGridRequest(
                yMin,
                chunk.FileCoordinateGridRequest.RowCount,
                xMin,
                xExtensionAmount + chunk.FileCoordinateGridRequest.CharacterCount,
                activeRequest.CancellationToken);

            nextEditor = nextEditor with
            {
                RowIndexOffset = combinedRequest.StartingRowIndex
            };

            lapKindTuples.Remove((LapKind.East, LapKindModifier.Extends));

            return chunk with
            {
                PlainTextEditorRecord = nextEditor,
                FileCoordinateGridRequest = combinedRequest
            };
        }

        private static PlainTextEditorChunk ExtendChunkSouth(FileCoordinateGridRequest activeRequest,
            PlainTextEditorChunk chunk,
            List<(LapKind lapKind, LapKindModifier lapKindModifier)> lapKindTuples)
        {
            // Non if statement required setting
            int yMin = chunk.FileCoordinateGridRequest.StartingRowIndex + chunk.FileCoordinateGridRequest.RowCount;
            int yExclusiveMax = activeRequest.StartingRowIndex + activeRequest.RowCount;
            int yExtensionAmount = yExclusiveMax - yMin;

            // Require if statement
            int xMin = chunk.FileCoordinateGridRequest.StartingCharacterIndex;
            int xExclusiveMax = chunk.FileCoordinateGridRequest.StartingCharacterIndex + chunk.FileCoordinateGridRequest.CharacterCount;
            int xExtensionAmount = 0;

            var westExtension = (LapKind.West, LapKindModifier.Extends);
            if (lapKindTuples.Contains(westExtension))
            {
                xMin = activeRequest.StartingCharacterIndex;

                xExtensionAmount += chunk.FileCoordinateGridRequest.StartingCharacterIndex - activeRequest.StartingCharacterIndex;
            }

            var eastExtension = (LapKind.East, LapKindModifier.Extends);
            if (lapKindTuples.Contains(eastExtension))
            {
                xExclusiveMax = activeRequest.StartingCharacterIndex + activeRequest.CharacterCount;

                xExtensionAmount += (chunk.FileCoordinateGridRequest.StartingCharacterIndex + chunk.FileCoordinateGridRequest.CharacterCount)
                                    - (activeRequest.StartingCharacterIndex + activeRequest.CharacterCount);
            }

            var southRequest = new FileCoordinateGridRequest(
                yMin,
                yExclusiveMax - yMin,
                xMin,
                xExclusiveMax - xMin,
                activeRequest.CancellationToken);

            var content = chunk.PlainTextEditorRecord.FileCoordinateGrid
                .Request(southRequest);

            var nextEditor = AlterChunk(chunk.PlainTextEditorRecord,
                content,
                southRequest,
                chunk.FileCoordinateGridRequest);

            var combinedRequest = new FileCoordinateGridRequest(
                yMin,
                yExtensionAmount + chunk.FileCoordinateGridRequest.RowCount,
                xMin,
                xExtensionAmount + chunk.FileCoordinateGridRequest.CharacterCount,
                activeRequest.CancellationToken);

            nextEditor = nextEditor with
            {
                RowIndexOffset = combinedRequest.StartingRowIndex
            };

            lapKindTuples.Remove((LapKind.South, LapKindModifier.Extends));

            return chunk with
            {
                PlainTextEditorRecord = nextEditor,
                FileCoordinateGridRequest = combinedRequest
            };
        }

        private static PlainTextEditorChunk ExtendChunkWest(FileCoordinateGridRequest activeRequest,
            PlainTextEditorChunk chunk,
            List<(LapKind lapKind, LapKindModifier lapKindModifier)> lapKindTuples)
        {
            // Non if statement required setting
            int xMin = activeRequest.StartingCharacterIndex;
            int xExclusiveMax = chunk.FileCoordinateGridRequest.StartingCharacterIndex;
            int xExtensionAmount = xExclusiveMax - xMin;

            int yMin = chunk.FileCoordinateGridRequest.StartingRowIndex;
            int yExclusiveMax = chunk.FileCoordinateGridRequest.StartingRowIndex + chunk.FileCoordinateGridRequest.RowCount;

            var westRequest = new FileCoordinateGridRequest(
                yMin,
                yExclusiveMax - yMin,
                xMin,
                xExclusiveMax - xMin,
                activeRequest.CancellationToken);

            var content = chunk.PlainTextEditorRecord.FileCoordinateGrid
                .Request(westRequest);

            var nextEditor = AlterChunk(chunk.PlainTextEditorRecord,
                content,
                westRequest,
                chunk.FileCoordinateGridRequest);

            var combinedRequest = new FileCoordinateGridRequest(
                yMin,
                chunk.FileCoordinateGridRequest.RowCount,
                xMin,
                xExtensionAmount + chunk.FileCoordinateGridRequest.CharacterCount,
                activeRequest.CancellationToken);

            nextEditor = nextEditor with
            {
                RowIndexOffset = combinedRequest.StartingRowIndex
            };

            lapKindTuples.Remove((LapKind.West, LapKindModifier.Extends));

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
                
                plainTextEditorRecord = StateMachine.HandleHome(plainTextEditorRecord,
                    new KeyDownEventRecord(
                        KeyboardKeyFacts.MovementKeys.HOME_KEY,
                        KeyboardKeyFacts.MovementKeys.HOME_KEY,
                        true,
                        false,
                        false));

                for (int moveRow = plainTextEditorRecord.CurrentRowIndex + plainTextEditorRecord.RowIndexOffset;
                     moveRow < correspondingChunkRowIndex;
                     moveRow++)
                {
                    plainTextEditorRecord = StateMachine.HandleArrowDown(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                            KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                            false,
                            false,
                            false));
                }

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

                    plainTextEditorRecord = StateMachine.HandleArrowDown(plainTextEditorRecord,
                        new KeyDownEventRecord(
                            KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                            KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY,
                            false,
                            false,
                            false));

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
