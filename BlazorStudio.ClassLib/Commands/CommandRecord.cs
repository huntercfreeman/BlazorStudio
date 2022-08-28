using BlazorStudio.ClassLib.Store.ContextCase;

namespace BlazorStudio.ClassLib.Commands;

/// <param name="CommandKey">Used to guarantee a unique identifier exists</param>
/// <param name="DisplayName">The name displayed to the user (perhaps in a ContextMenu)</param>
/// <param name="CommandName">A name that is more-so internally used as a less friendly, but perhaps more succinct or descriptive name than <see cref="DisplayName"/></param>
public record CommandRecord(CommandKey CommandKey, string DisplayName, string CommandName, Func<KeymapEventAction, object> ActionToDispatchConstructor);