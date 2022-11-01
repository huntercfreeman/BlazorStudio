using BlazorStudio.ClassLib.CustomEvents;
using BlazorStudio.ClassLib.TreeView;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TreeView;

public class TreeViewDisplayOnEventRegistration<TItem>
{
    public Func<CustomKeyDownEventArgs, TreeViewDisplay<TItem>, Task>? 
        AfterKeyDownFuncAsync { get; set; }
    
    public Func<MouseEventArgs, TreeViewDisplay<TItem>, Task>? 
        AfterClickFuncAsync { get; set; }
    
    public Func<MouseEventArgs, TreeViewDisplay<TItem>, Task>? 
        AfterDoubleClickFuncAsync { get; set; }
}
