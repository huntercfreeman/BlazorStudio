using BlazorStudio.ClassLib.Family;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : FluxorLayout
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private TreeViewWrapKey _familyTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrapKey _themeTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();

    private List<Person> _rootPeople = GetRootPeople();

    private List<ThemeKey> _rootThemes = GetRootThemes();

    private static List<Person> GetRootPeople()
    {
        var children = new List<Person>
        {
            new("OneChild", "Francisco"),
            new("TwoChild", "Francisco"),
            new("ThreeChild", "Francisco"),
        };

        var rootPeople = new List<Person>
        {
            new("Bob", "Francisco")
            {
                Children = children
            },
            new("Lisa", "Francisco")
            {
                Children = children
            }
        };

        return rootPeople;
    }
    
    private Task<IEnumerable<Person>> LoadPersonChildren(Person person)
    {
        return Task.FromResult(person.Children.AsEnumerable());
    }
    
    private static List<ThemeKey> GetRootThemes()
    {
        return ThemeFacts.AllDefaultThemeKeys.ToList();
    }
    
    private Task<IEnumerable<ThemeKey>> LoadThemesChildren(ThemeKey themeKey)
    {
        return Task.FromResult(Array.Empty<ThemeKey>().AsEnumerable());
    }
    
    private void FamilyTreeViewOnEnterKeyDown(Person person)
    {
    }

    private void FamilyTreeViewOnSpaceKeyDown(Person person)
    {
    }
    
    private void ThemeTreeViewOnEnterKeyDown(ThemeKey themeKey)
    {
        Dispatcher.Dispatch(new SetThemeStateAction(themeKey));
    }

    private void ThemeTreeViewOnSpaceKeyDown(ThemeKey themeKey)
    {
    }
}