using BlazorStudio.ClassLib.CustomEvents;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.TreeView;
using BlazorTextEditor.RazorLib.Store.ThemeCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using ThemeFacts = BlazorStudio.ClassLib.Store.ThemeCase.ThemeFacts;

namespace BlazorStudio.RazorLib.Theme;

public partial class ThemeSelectTreeView : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private TreeViewKey _themeTreeViewKey = TreeViewKey.NewTreeViewKey();

    private TreeViewModel<ThemeRecord> GetRoot()
    {
        return new TreeViewModel<ThemeRecord>(
            null,
            LoadTreeViewModelChildren);
    }

    private Task LoadTreeViewModelChildren(TreeViewModel<ThemeRecord> treeViewModel)
    {
        var children = ThemeFacts.DefaultThemeRecords
            .Select(x => new TreeViewModel<ThemeRecord>(
                x,
                LoadTreeViewModelChildren));
                
        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(children);
    }
}