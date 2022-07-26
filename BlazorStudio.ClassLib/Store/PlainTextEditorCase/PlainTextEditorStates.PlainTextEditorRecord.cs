using System.Collections.Immutable;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Virtualize;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    private record PlainTextEditorRecord(PlainTextEditorKey PlainTextEditorKey,
            SequenceKey SequenceKey,
            ImmutableList<IPlainTextEditorRow> List,
            int CurrentRowIndex,
            int CurrentTokenIndex,
            IFileCoordinateGrid? FileCoordinateGrid,
            RichTextEditorOptions RichTextEditorOptions,
            bool IsReadonly = true,
            bool UseCarriageReturnNewLine = false)
        : IPlainTextEditor
    {
        public PlainTextEditorRecord(PlainTextEditorKey plainTextEditorKey) : this(plainTextEditorKey,
            SequenceKey.NewSequenceKey(),
            ImmutableList<IPlainTextEditorRow>.Empty,
            CurrentRowIndex: 0,
            CurrentTokenIndex: 0,
            null,
            new RichTextEditorOptions())
        {
            List = List.Add(GetEmptyPlainTextEditorRow());
        }

        public IPlainTextEditorRow CurrentPlainTextEditorRow => List[CurrentRowIndex];

        public TextTokenKey CurrentTextTokenKey => CurrentPlainTextEditorRow.List[CurrentTokenIndex].Key;
        public ITextToken CurrentTextToken => CurrentPlainTextEditorRow.List[CurrentTokenIndex];
        public int LongestRowCharacterLength { get; init; }
        public VirtualizeCoordinateSystemMessage VirtualizeCoordinateSystemMessage { get; init; }
        public int RowIndexOffset { get; init; }
        public int CharacterIndexOffsetRelativeToRow { get; init; }
        public List<PlainTextEditorChunk> Cache { get; init; } = new();
        public int CachedChunkIndex { get; private set; }
        public int CacheCount => Cache.Count;

        public T GetCurrentTextTokenAs<T>()
            where T : class
        {
            return CurrentTextToken as T
                   ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }

        /// <summary>
        /// TODO: Remove this and in its place use <see cref="ConvertIPlainTextEditorRowAs"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public T GetCurrentPlainTextEditorRowAs<T>()
            where T : class
        {
            return CurrentPlainTextEditorRow as T
                   ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }

        public T ConvertIPlainTextEditorRowAs<T>(IPlainTextEditorRow plainTextEditorRow)
            where T : class
        {
            return plainTextEditorRow as T
                   ?? throw new ApplicationException($"Expected {typeof(T).Name}");
        }

        public string GetPlainText()
        {
            var cache = Cache[CachedChunkIndex];
            var cacheRowList = cache.PlainTextEditorRecord.List;

            var builder = new StringBuilder();

            foreach (var row in cacheRowList)
            {
                foreach (var token in row.List)
                {
                    if (token.Key == cacheRowList[0].List[0].Key)
                    {
                        // Is first start of row so skip
                        // as it would insert a enter key stroke at start
                        // of document otherwise.

                        continue;
                    }

                    builder.Append(token.CopyText);
                }
            }

            return builder.ToString();
        }
        
        public IPlainTextEditorRow GetEmptyPlainTextEditorRow()
        {
            return new PlainTextEditorRow(null);
        }
        
        public void SetCache(PlainTextEditorRecord plainTextEditorRecord)
        {
            var previousCache = Cache[CachedChunkIndex];

            Cache[CachedChunkIndex] = previousCache with
            {
                PlainTextEditorRecord = plainTextEditorRecord
            };
        }

        // TODO: This seems be good progress and I will keep it but I need to revisit this and create a sort of 'LinkedList' like structure or something 'new line' messes everything up I think?
        public PlainTextEditorChunk UpdateCache(FileCoordinateGridRequest fileCoordinateGridRequest)
        {
            int lastIndexOfOverlap = -1;
            
            for (var index = 0; index < Cache.Count; index++)
            {
                var cachedChunk = Cache[index];

                // Search chunk for any overlapping characters.
                // Overlapping characters will EXTEND that overlapping chunk
                if (cachedChunk.OverlapsRequest(fileCoordinateGridRequest,
                        out var chunk))
                {
                    Cache[index] = chunk;

                    // In the case that the request is 'sandwiched' between
                    // between two chunks AND overlaps both sandwiching chunks
                    // one cannot return immediately and must allow all overlaps to merge.
                    lastIndexOfOverlap = index;
                }
            }

            PlainTextEditorChunk resultingChunk;

            if (lastIndexOfOverlap >= 0)
            {
                // An existing chunk was overlapping the request

                resultingChunk = Cache[lastIndexOfOverlap];

                CachedChunkIndex = lastIndexOfOverlap;
            }
            else
            {
                // If there are no chunks that overlap then a NEW chunk is made
                var content = FileCoordinateGrid
                    .Request(fileCoordinateGridRequest);

                var constructedPlainTextEditor = this with
                {
                    CurrentRowIndex = 0,
                    CurrentTokenIndex = 0,
                    SequenceKey = SequenceKey.NewSequenceKey(),
                    List = ImmutableList<IPlainTextEditorRow>.Empty
                        .Add(GetEmptyPlainTextEditorRow()),
                };

                resultingChunk = ConstructChunk(constructedPlainTextEditor,
                    content,
                    fileCoordinateGridRequest);

                constructedPlainTextEditor.Cache.Add(resultingChunk);

                // TODO: This is sketchy because of threading concerns however, the Reducer is synchronous as of writing this
                CachedChunkIndex = Cache.Count - 1;
            }

            return resultingChunk;
        }

        private static PlainTextEditorChunk ConstructChunk(PlainTextEditorRecord plainTextEditorRecord,
            List<string> content,
            FileCoordinateGridRequest fileCoordinateGridRequest)
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

            return new PlainTextEditorChunk(
                fileCoordinateGridRequest,
                content,
                plainTextEditorRecord);
        }
    }
}