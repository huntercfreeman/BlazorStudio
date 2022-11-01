namespace BlazorStudio.ClassLib.Context;

public record ContextRecord(
    ContextKey ContextKey,
    string DisplayNameFriendly,
    string ContextNameInternal);