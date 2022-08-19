using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.KeymapCase;

namespace BlazorStudio.ClassLib.Contexts;

/// <summary>
/// Used to handle a keymap event <see cref="KeymapState"/>
/// differently depending on where the user's active focus is within the
/// application.
/// </summary>
/// <param name="ContextKey">Used to guarantee a unique identifier exists</param>
/// <param name="DisplayName">The name displayed to the user (perhaps it could be, "TextEditor", "Global", "SolutionExplorer")</param>
/// <param name="ContextName">A name that is more-so internally used as a less friendly, but perhaps more succinct or descriptive name than <see cref="DisplayName"/></param>
public record ContextRecord(ContextKey ContextKey, string DisplayName, string ContextName, Keymap Keymap);