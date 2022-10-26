using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Clipboard;
using BlazorTextEditor.RazorLib.Commands;
using BlazorTextEditor.RazorLib.Cursor;
using BlazorTextEditor.RazorLib.HelperComponents;
using BlazorTextEditor.RazorLib.TextEditor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.Editor;

public partial class TextEditorContextMenu : ComponentBase
{
    private ElementReference? _textEditorContextMenuElementReference;
    [Inject]
    private IClipboardProvider ClipboardProvider { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter(Name = "SetShouldDisplayMenuAsync")]
    public Func<TextEditorMenuKind, Task> SetShouldDisplayMenuAsync { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public TextEditorDisplay TextEditorDisplay { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;

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
                catch (JSException)
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
            await SetShouldDisplayMenuAsync.Invoke(TextEditorMenuKind.None);
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

    private async Task CopyMenuOption()
    {
        var cursorSnapshots = new TextEditorCursorSnapshot[]
        {
            new(TextEditorDisplay.PrimaryCursor),
        }.ToImmutableArray();

        await TextEditorCommandFacts.Copy.DoAsyncFunc
            .Invoke(new TextEditorCommandParameter(
                TextEditor,
                cursorSnapshots,
                ClipboardProvider,
                TextEditorService,
                TextEditorDisplay.ReloadVirtualizationDisplay,
                TextEditorDisplay.OnSaveRequested));
    }

    private async Task PasteMenuOption()
    {
        var cursorSnapshots = new TextEditorCursorSnapshot[]
        {
            new(TextEditorDisplay.PrimaryCursor),
        }.ToImmutableArray();

        await TextEditorCommandFacts.Paste.DoAsyncFunc
            .Invoke(new TextEditorCommandParameter(
                TextEditor,
                cursorSnapshots,
                ClipboardProvider,
                TextEditorService,
                TextEditorDisplay.ReloadVirtualizationDisplay,
                TextEditorDisplay.OnSaveRequested));
    }
}