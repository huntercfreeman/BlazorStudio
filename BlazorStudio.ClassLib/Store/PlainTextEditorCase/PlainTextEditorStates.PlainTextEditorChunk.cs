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

            /*
             * # Sample text as an example with line numbers
             *
             *  1: Since the release of C# 2.0 in November 2005, the C# and Java languages have evolved on increasingly divergent
             *  2: trajectories, becoming two quite different languages. One of the first major departures came with the addition of generics
             *  3: to both languages, with vastly different implementations. C# makes use of reification to provide "first-class" generic
             *  4: objects that can be used like any other class, with code generation performed at class-load time.[29] Furthermore, C#
             *  5: has added several major features to accommodate functional-style programming, culminating in the LINQ extensions
             *  6: released with C# 3.0 and its supporting framework of lambda expressions, extension methods, and anonymous types.[30]
             *  7: These features enable C# programmers to use functional programming techniques, such as closures, when it is advantageous
             *  8: to their application. The LINQ extensions and the functional imports help developers reduce the amount of boilerplate
             *  9: code that is included in common tasks like querying a database, parsing an xml file, or searching through a data structure,
             * 10: shifting the emphasis onto the actual program logic to help improve readability and maintainability.[31]
             * 11: C# used to have a mascot called Andy (named after Anders Hejlsberg). It was retired on January 29, 2004.[32]
             * 12: C# was originally submitted to the ISO/IEC JTC 1 subcommittee SC 22 for review,[33] under ISO/IEC 23270:2003,[34]
             * 13: was withdrawn and was then approved under ISO/IEC 23270:2006.[35] The 23270:2006 is withdrawn under 23270:2018 and
             * 14: approved with this version.[36]
             *
             * # Chunk 0
             *
             * John went to the store and
             * came back with food
             * for dinner.
             */

            /*
             * # Chunk 0
             *
             * John went to the store and
             * came back with food
             * for dinner.
             */


            if (chunkInclusiveStartingRowIndex < currentRequestInclusiveStartingRowIndex ||
                (chunkInclusiveStartingRowIndex == currentRequestInclusiveStartingRowIndex 
                    && chunkInclusiveStartingCharacterIndex < currentRequestInclusiveStartingCharacterIndex))
            {
                // If the chunk has content that comes BEFORE the currentRequest

                if (chunkExclusiveEndingRowIndex <= currentRequestExclusiveEndingRowIndex)
                {
                    // If the chunk has content that OVERLAPS the currentRequest

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

                }
            }
        }
    }
}
