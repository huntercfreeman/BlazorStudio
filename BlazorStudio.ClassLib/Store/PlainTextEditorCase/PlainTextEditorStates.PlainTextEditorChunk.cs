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
            var activeRequestLapMaker = GetLapKindTuples(chunkCoordinates, 
                activeRequestCoordinates);

            if (!activeRequestLapMaker.Any())
            {
                // No overlapping
                outPlainTextEditorChunk = null;
                return false;
            }

            // TODO: Calculations
            outPlainTextEditorChunk = null;
            return false;
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

            int availableRowCount = subrequest.RowCount;

            if (content.Count < subrequest.RowCount)
            {
                availableRowCount = content.Count;
            }

            for (int i = subrequest.StartingRowIndex;
                 i < availableRowCount;
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
                        CurrentRowIndex = correspondingChunkRowIndex,
                        CurrentTokenIndex = 0
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
