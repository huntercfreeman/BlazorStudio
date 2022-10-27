using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorTextEditor.RazorLib.HelperComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Editor;

public partial class TextEditorAutoCompleteMenu : ComponentBase
{
    private ElementReference? _textEditorAutoCompleteMenuElementReference;
    [CascadingParameter(Name = "SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, Task> SetShouldDisplayMenuAsync { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public string AutoCompleteWordText { get; set; } = string.Empty;

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key)
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None);
    }

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords()
    {
        List<MenuOptionRecord> menuOptionRecords = new();

        var copy = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
            AutoCompleteWordText,
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
            : new[]
            {
                new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                    "No Context Menu Options for this item",
                    ImmutableList<MenuOptionRecord>.Empty,
                    null,
                    MenuOptionKind.Read),
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

    private Task CopyMenuOption()
    {
        return Task.CompletedTask;
    }

    private Task PasteMenuOption()
    {
        return Task.CompletedTask;
    }
}