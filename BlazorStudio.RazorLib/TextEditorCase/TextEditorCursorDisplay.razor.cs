using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorCursorDisplay : ComponentBase
{
    [Parameter]
    public TextCursor TextCursor { get; set; } = null!;
    [Parameter]
    public TextEditorBase TextEditorBase { get; set; } = null!;
    [Parameter]
    public double RowHeightInPixels { get; set; }
    [Parameter]
    public double CharacterWidthInPixels { get; set; }

    private ElementReference? _textAreaCursorDisplayElementReference;

    private string CssStyleString => $"top: {TextCursor.IndexCoordinates.RowIndex.Value * RowHeightInPixels}px;" +
                                     " " +
                                     $"left: {GetCssStylingLeft()}px;";

    private string GetCssStylingLeft()
    {
        var baseWidth = TextCursor.IndexCoordinates.ColumnIndex.Value * CharacterWidthInPixels;
        
        // Tab keys for example can have a width > 1 character and must offset the cursor position accordingly.
        var tabsOnSameRowBeforeCursor =TextEditorBase.GetTabsCountOnSameRowBeforeCursor(new ImmutableTextCursor(TextCursor));
        var offsetWidth = tabsOnSameRowBeforeCursor * ((TextEditorBase.TabWidth - 1) * CharacterWidthInPixels);

        var mostDigitsInALineNumber = TextEditorBase.LineEndingPositions.Length
            .ToString().Length;
        
        var offsetLineNumbers = mostDigitsInALineNumber * CharacterWidthInPixels;
        
        var marginLeft = TextEditorBase.FileContentMarginLeftSeparateFromLineNumbers * CharacterWidthInPixels;
        
        return (baseWidth + offsetWidth + offsetLineNumbers + marginLeft)
            .ToString();
    }

    public void HandleKeyboardEvent(KeyboardEventArgs keyboardEventArgs)
    {
        TextCursor.MoveCursor(keyboardEventArgs, TextCursor, TextEditorBase);
        
        StateHasChanged();
    }

    public async Task FocusAsync()
    {
        if (_textAreaCursorDisplayElementReference is not null)
        {
            await _textAreaCursorDisplayElementReference.Value.FocusAsync();
        }
    }
}