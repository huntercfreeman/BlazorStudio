using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TreeViewCase;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record TreeViewContextMenuEventDto<T>(MouseEventArgs? MouseEventArgs,
        // ReSharper disable once NotAccessedPositionalProperty.Global
        KeyboardEventArgs? KeyboardEventArgs,
        T Item,
        Action? ToggleIsExpanded,
        Action? SetIsActive,
        Func<Task> RefreshContextMenuTarget,
        Func<Task>? RefreshParentOfContextMenuTarget,
        ElementReference FocusAfterTarget)
    : TreeViewEventDto<T>(Item,
        ToggleIsExpanded,
        SetIsActive,
        RefreshContextMenuTarget,
        RefreshParentOfContextMenuTarget);