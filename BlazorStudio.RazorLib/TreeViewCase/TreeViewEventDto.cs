namespace BlazorStudio.RazorLib.TreeViewCase;

public record TreeViewEventDto<T>(T Item,
    Action ToggleIsExpanded,
    Action SetIsActive,
    Func<Task> RefreshContextMenuTarget,
    Func<Task> RefreshParentOfContextMenuTarget);