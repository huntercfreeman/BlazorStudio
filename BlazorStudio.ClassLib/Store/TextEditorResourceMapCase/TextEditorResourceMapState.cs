using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorResourceMapCase;

[FeatureState]
public record TextEditorResourceMapState(ImmutableDictionary<TextEditorKey, IAbsoluteFilePath> ResourceMap)
{
    public TextEditorResourceMapState() : this(ImmutableDictionary<TextEditorKey, IAbsoluteFilePath>.Empty)
    {
        
    }
}