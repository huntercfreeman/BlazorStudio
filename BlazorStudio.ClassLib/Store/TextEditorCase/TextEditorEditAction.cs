using System.Collections.Immutable;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.Enums;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

/// <param name="TextCursorTuples">
/// Contains <see cref="ImmutableTextCursor"/> as to ensure
/// a snapshot of where the end user's cursor was is taken. Thereby it will be perfectly placed even if they move the cursor
/// before the edit finishes.
/// <br/><br/>
/// When editing text the <see cref="TextCursor"/> may need position updates.
/// </param>
public record TextEditorEditAction(
    TextEditorKey TextEditorKey,
    ImmutableArray<(ImmutableTextCursor immutableTextCursor, TextCursor textCursor)> TextCursorTuples,
    KeyboardEventArgs KeyboardEventArgs,
    CancellationToken CancellationToken);
    
    