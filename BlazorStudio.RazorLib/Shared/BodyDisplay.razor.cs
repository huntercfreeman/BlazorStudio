using BlazorStudio.ClassLib.Family;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class BodyDisplay : ComponentBase
{
    private TreeViewWrapKey _familyTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private List<Person> _rootPeople = GetRootPeople();

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

    private void FamilyTreeViewOnEnterKeyDown(Person person)
    {
    }

    private void FamilyTreeViewOnSpaceKeyDown(Person person)
    {
    }
}