using BlazorStudio.ClassLib.Store.TreeViewCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase
{
    private TreeViewKey _familyTreeViewKey = TreeViewKey.NewTreeViewKey();
    private TreeViewKey _themeTreeViewKey = TreeViewKey.NewTreeViewKey();
}