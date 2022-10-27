using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using ThemeFacts = BlazorStudio.ClassLib.Store.ThemeCase.ThemeFacts;

namespace BlazorStudio.RazorLib.Theme;

public partial class ThemeSelectTreeView : ComponentBase
{
    private List<ThemeKey> _rootThemes = GetRootThemes();

    private TreeViewWrapKey _themeTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private static List<ThemeKey> GetRootThemes()
    {
        return ThemeFacts.AllDefaultThemeKeys.ToList();
    }

    private Task<IEnumerable<ThemeKey>> LoadThemesChildren(ThemeKey themeKey)
    {
        return Task.FromResult(Array.Empty<ThemeKey>().AsEnumerable());
    }

    private void ThemeTreeViewOnEnterKeyDown(TreeViewKeyboardEventDto<ThemeKey> treeViewKeyboardEventDto)
    {
        Dispatcher.Dispatch(new SetThemeStateAction(treeViewKeyboardEventDto.Item));
    }

    private void ThemeTreeViewOnSpaceKeyDown(TreeViewKeyboardEventDto<ThemeKey> treeViewKeyboardEventDto)
    {
        ThemeTreeViewOnEnterKeyDown(treeViewKeyboardEventDto);
    }

    private void ThemeTreeViewOnDoubleClick(TreeViewMouseEventDto<ThemeKey> treeViewMouseEventDto)
    {
        Dispatcher.Dispatch(new SetThemeStateAction(treeViewMouseEventDto.Item));
    }
}