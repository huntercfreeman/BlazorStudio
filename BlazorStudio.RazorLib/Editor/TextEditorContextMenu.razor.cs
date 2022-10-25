using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.RazorLib.Forms;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.Store.TextEditorCase;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class TextEditorContextMenu : ComponentBase
{
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [CascadingParameter(Name="SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, Task> SetShouldDisplayMenuAsync { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorDisplay TextEditorDisplay { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    
    private ElementReference? _textEditorContextMenuElementReference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_textEditorContextMenuElementReference is not null)
            {
                try
                {
                    await _textEditorContextMenuElementReference.Value
                        .FocusAsync();
                }
                catch (Microsoft.JSInterop.JSException)
                {
                    // Caused when calling:
                    // await (ElementReference).FocusAsync();
                    // After component is no longer rendered
                }
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
        {
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None);
        }
    }
    
    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords()
    {
        List<MenuOptionRecord> menuOptionRecords = new();
        
        var copy = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
            "Copy",
            ImmutableList<MenuOptionRecord>.Empty,
            () => SelectMenuOption(CopyMenuOption),
            MenuOptionKind.Update);

        menuOptionRecords.Add(copy);
        
        var paste = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
            "Paste",
            ImmutableList<MenuOptionRecord>.Empty,
            () => SelectMenuOption(PasteMenuOption),
            MenuOptionKind.Update);
                
        menuOptionRecords.Add(paste);

        return menuOptionRecords.Any()
            ? menuOptionRecords
            : new []
            {
                new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                    "No Context Menu Options for this item",
                    ImmutableList<MenuOptionRecord>.Empty, 
                    null,
                    MenuOptionKind.Read)
            };
    }

    private void SelectMenuOption(Func<Task> menuOptionAction)
    {
        _ = Task.Run(async () =>
        {
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None);
            await menuOptionAction();
        });
    }
    
    private async Task CopyMenuOption()
    {
        var result = TextEditorDisplay.PrimaryCursor
            .GetSelectedText(TextEditor);
                
        if (result is not null)
            await ClipboardProvider.SetClipboard(result);
    }
    
    private async Task PasteMenuOption()
    {
        var clipboard = await ClipboardProvider.ReadClipboard();

        var previousCharacterWasCarriageReturn = false;
        
        foreach (var character in clipboard)
        {
            if (previousCharacterWasCarriageReturn &&
                character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                previousCharacterWasCarriageReturn = false;
                continue;
            }
            
            var code = character switch
            {
                '\r' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                '\n' => KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
                '\t' => KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
                ' ' => KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
                _ => character.ToString()
            };
 
            TextEditorService.EditTextEditor(new EditTextEditorBaseAction(TextEditorDisplay.TextEditorKey,
                new (ImmutableTextEditorCursor, TextEditorCursor)[]
                {
                    (new ImmutableTextEditorCursor(TextEditorDisplay.PrimaryCursor), TextEditorDisplay.PrimaryCursor)
                }.ToImmutableArray(),
                new KeyboardEventArgs
                {
                    Code = code,
                    Key = character.ToString()
                },
                CancellationToken.None));

            previousCharacterWasCarriageReturn = KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN
                                                 == character;
        }

        TextEditorDisplay.ReloadVirtualizationDisplay();
    }
}