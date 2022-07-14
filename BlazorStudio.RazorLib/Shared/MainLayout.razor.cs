using BlazorStudio.ClassLib.Family;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase
{
    private TreeViewWrapKey _familyTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrapKey _themeTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();

    private List<Person> _rootPeople = new List<Person>
    {
        new Person(PersonKey.NewPersonKey(), "Bob", "Francisco"),
        new Person(PersonKey.NewPersonKey(), "Lisa", "Francisco")
    };
}