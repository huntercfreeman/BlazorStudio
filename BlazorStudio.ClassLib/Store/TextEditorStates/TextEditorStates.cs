using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor;

namespace BlazorStudio.ClassLib.Store.TextEditorStates;

public record TextEditorStates(ImmutableDictionary<TextEditorKey, TextEditorBase> TextEditorMap);