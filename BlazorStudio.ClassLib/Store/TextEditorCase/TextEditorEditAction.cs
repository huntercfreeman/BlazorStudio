using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.Enums;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

public record TextEditorEditAction(
    TextEditorKey TextEditorKey,
    ImmutableArray<ImmutableTextCursor> ImmutableTextCursors,
    KeyboardEventArgs KeyboardEventArgs,
    CancellationToken CancellationToken);
    
    