using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Theme;

public partial class ThemeSelectTreeView : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private TreeViewWrapKey _themeTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private List<ThemeKey> _rootThemes = GetRootThemes();

    private static List<ThemeKey> GetRootThemes()
    {
        return ThemeFacts.AllDefaultThemeKeys.ToList();
    }

    private Task<IEnumerable<ThemeKey>> LoadThemesChildren(ThemeKey themeKey)
    {
        return Task.FromResult(Array.Empty<ThemeKey>().AsEnumerable());
    }

    private void ThemeTreeViewOnEnterKeyDown(ThemeKey themeKey, Action toggleIsExpanded)
    {
        Dispatcher.Dispatch(new SetThemeStateAction(themeKey));
    }

    private void ThemeTreeViewOnSpaceKeyDown(ThemeKey themeKey, Action toggleIsExpanded)
    {
    }

    private void ThemeTreeViewOnDoubleClick(ThemeKey themeKey, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    {
        Dispatcher.Dispatch(new SetThemeStateAction(themeKey));
    }
}