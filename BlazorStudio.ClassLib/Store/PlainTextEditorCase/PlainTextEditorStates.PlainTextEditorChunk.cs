using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorChunk(PlainTextEditorRowKey Key,
        FileCoordinateGridRequest FileCoordinateGridRequest,
        List<string> ContentOfRows)
    {
        public FileCoordinateGridRequest SearchChunk(FileCoordinateGridRequest currentRequest,
            out List<string> contentFoundInCache)
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

                if ()
                {
                    // If the chunk has content that OVERLAPS the currentRequest

                }
            }
            else if (chunkExclusiveEndingRowIndex > currentRequestExclusiveEndingRowIndex ||
                     (chunkExclusiveEndingRowIndex == currentRequestExclusiveEndingRowIndex
                      && chunkExclusiveEndingCharacterIndex < currentRequestExclusiveEndingCharacterIndex))
            {
                // If the chunk has content that comes AFTER the currentRequest

                if ()
                {
                    // If the chunk has content that OVERLAPS the currentRequest

                }
            }
        }
    }
}
