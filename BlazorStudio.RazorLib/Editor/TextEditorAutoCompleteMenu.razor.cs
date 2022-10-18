using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorTextEditor.RazorLib.HelperComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class TextEditorAutoCompleteMenu : ComponentBase
{
    [CascadingParameter(Name="SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, Task> SetShouldDisplayMenuAsync { get; set; } = null!;
    
    private ElementReference? _textEditorAutoCompleteMenuElementReference;

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
            "AutoComplete 1",
            ImmutableList<MenuOptionRecord>.Empty,
            () => SelectMenuOption(CopyMenuOption),
            MenuOptionKind.Update);

        menuOptionRecords.Add(copy);
        
        var paste = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
            "AutoComplete 2",
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