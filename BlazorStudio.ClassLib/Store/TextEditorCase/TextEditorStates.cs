using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(
    ImmutableDictionary<TextEditorKey, TextEditorBase> TextEditorMap,
    ImmutableDictionary<AbsoluteFilePathStringValue, TextEditorKey> AbsoluteFilePathToActiveTextEditorMap)
{
    public TextEditorStates() : this(
        ImmutableDictionary<TextEditorKey, TextEditorBase>.Empty,
        ImmutableDictionary<AbsoluteFilePathStringValue, TextEditorKey>.Empty)
    {
        
    }
}