using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(
    ImmutableList<TextEditorBase> TextEditors,
    ImmutableDictionary<AbsoluteFilePathStringValue, TextEditorKey> AbsoluteFilePathToActiveTextEditorMap)
{
    public TextEditorStates() : this(
        ImmutableList<TextEditorBase>.Empty,
        ImmutableDictionary<AbsoluteFilePathStringValue, TextEditorKey>.Empty)
    {
        
    }
}