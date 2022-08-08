using System.Collections.Immutable;
using System.Runtime.InteropServices;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Virtualize;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.ClassLib.Store.PlainTextEditorCase;

public partial record PlainTextEditorStates
{
    public class PlainTextEditorStatesReducer
    {
        [ReducerMethod]
        public static PlainTextEditorStates ReduceSetPlainTextEditorStatesAction(PlainTextEditorStates previousPlainTextEditorStates,
            SetPlainTextEditorStatesAction setPlainTextEditorStatesAction)
        {
            return setPlainTextEditorStatesAction.PlainTextEditorStates;
        }
        
        [ReducerMethod]
        public static PlainTextEditorStates ReducePlainTextEditorSetFontSizeAction(PlainTextEditorStates previousPlainTextEditorStates,
            PlainTextEditorSetFontSizeAction plainTextEditorSetFontSizeAction)
        {
            var nextPlainTextEditorMap =
                new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates
                    .Map[plainTextEditorSetFontSizeAction.PlainTextEditorKey]
                as PlainTextEditorRecordBase;

            if (plainTextEditor is null)
                return previousPlainTextEditorStates;

            var nextPlainTextEditor = plainTextEditor with
            {
                RichTextEditorOptions = new RichTextEditorOptions
                {
                    FontSizeInPixels = plainTextEditorSetFontSizeAction.FontSize
                },
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            nextPlainTextEditorMap[plainTextEditorSetFontSizeAction.PlainTextEditorKey] =
                nextPlainTextEditor;

            var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
            var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

            return new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);
        }
        
        [ReducerMethod]
        public static PlainTextEditorStates ReducePlainTextEditorSetOptionsAction(PlainTextEditorStates previousPlainTextEditorStates,
            PlainTextEditorSetOptionsAction plainTextEditorSetOptionsAction)
        {
            var nextPlainTextEditorMap =
                new Dictionary<PlainTextEditorKey, IPlainTextEditor>(previousPlainTextEditorStates.Map);
            var nextPlainTextEditorList = new List<PlainTextEditorKey>(previousPlainTextEditorStates.Array);

            var plainTextEditor = previousPlainTextEditorStates
                    .Map[plainTextEditorSetOptionsAction.PlainTextEditorKey]
                as PlainTextEditorRecordBase;

            if (plainTextEditor is null)
                return previousPlainTextEditorStates;

            var nextPlainTextEditor = plainTextEditor with
            {
                RichTextEditorOptions = plainTextEditorSetOptionsAction.RichTextEditorOptions,
                SequenceKey = SequenceKey.NewSequenceKey()
            };

            nextPlainTextEditorMap[plainTextEditorSetOptionsAction.PlainTextEditorKey] =
                nextPlainTextEditor;

            var nextImmutableMap = nextPlainTextEditorMap.ToImmutableDictionary();
            var nextImmutableArray = nextPlainTextEditorList.ToImmutableArray();

            return new PlainTextEditorStates(nextImmutableMap, nextImmutableArray);
        }
    }
}

