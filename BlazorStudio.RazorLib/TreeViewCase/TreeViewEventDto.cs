namespace BlazorStudio.RazorLib.TreeViewCase;

public record TreeViewEventDto<T>(T Item,
    Action? ToggleIsExpanded,
    // ReSharper disable once NotAccessedPositionalProperty.Global
    Action? SetIsActive,
    Func<Task> RefreshContextMenuTarget,
    Func<Task>? RefreshParentOfContextMenuTarget);