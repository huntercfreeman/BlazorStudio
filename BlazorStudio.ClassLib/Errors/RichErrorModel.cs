namespace BlazorStudio.ClassLib.Errors;

/// <summary>
///     Construct a List of this type and add any issues that occur
///     on the UI to the List as this class type.
///     Remove from the List any of this class type which are resolved
///     at some point using the IsResolved Func to check if it is resolved,
///     then the OnIsResolvedAction to perform List.Remove(object)
/// </summary>
public class RichErrorModel
{
    public RichErrorModel(string message, string hint)
    {
        Message = message;
        Hint = hint;
    }

    public RichErrorModel(string message, string hint, Func<bool> isResolved, Action<RichErrorModel> onIsResolvedAction)
    {
        Message = message;
        Hint = hint;
        IsResolved = isResolved;
        OnIsResolvedAction = onIsResolvedAction;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public string Message { get; }
    public string Hint { get; }
    public Func<bool>? IsResolved { get; }
    public Action<RichErrorModel>? OnIsResolvedAction { get; }
}