namespace BlazorStudio.ClassLib.Git;

public enum GitDirtyReason
{
    None,
    Untracked,
    Added,
    Modified,
    Deleted,
}