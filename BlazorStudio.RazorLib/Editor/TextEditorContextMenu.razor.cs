using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.RazorLib.Forms;
using BlazorTextEditor.RazorLib.HelperComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class TextEditorContextMenu : ComponentBase
{
    [CascadingParameter(Name="SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, Task> SetShouldDisplayMenuAsync { get; set; } = null!;
    
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
        
    }
    
    private async Task PasteMenuOption()
    {
        
    }
}