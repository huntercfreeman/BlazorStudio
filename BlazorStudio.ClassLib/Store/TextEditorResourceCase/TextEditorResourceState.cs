using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.RazorLib.MoveThese;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorResourceCase;

[FeatureState]
public record TextEditorResourceState(ImmutableDictionary<TextEditorKey, IAbsoluteFilePath> ResourceMap)
{
    public TextEditorResourceState() : this(ImmutableDictionary<TextEditorKey, IAbsoluteFilePath>.Empty)
    {
    
    }
}