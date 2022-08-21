using BlazorStudio.RazorLib.CustomEvents;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TreeViewCase;

public record TreeViewKeyboardEventDto<T>(CustomKeyDown CustomKeyDown,
        T Item,
        Action ToggleIsExpanded,
        Action SetIsActive,
        Func<Task> RefreshContextMenuTarget,
        Func<Task> RefreshParentOfContextMenuTarget)
    : TreeViewEventDto<T>(Item, 
        ToggleIsExpanded, 
        SetIsActive,
        RefreshContextMenuTarget, 
        RefreshParentOfContextMenuTarget);