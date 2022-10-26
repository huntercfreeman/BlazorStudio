using System.Collections.Immutable;
using BlazorStudio.ClassLib.Commands;

namespace BlazorStudio.ClassLib.Keyboard;

/// <summary>
/// The closest ancestor context tries to handle the keymap event first.
///<br/>--<br/>
/// If the closest ancestor did not have a keymap for that keyboard input
/// then the event can bubble up to the next closest ancestor until
/// an ancestor handles the event.
///<br/>--<br/>
/// This works opposite to an HTML onkeydown event. HTML onkeydown event
/// will bubble up until there is no ancestor or if a handler
/// calls 'stopPropagation' or various other ways to achieve a similar result.
///<br/>--<br/>
/// The Keymap will automatically in effect do an HTML 'stopPropagation' once
/// a context had a valid keymap for the keyboard input.
///<br/>--<br/>
/// You instead must do almost the opposite of HTML and specify when you WANT to propagate the event
/// even though you handled it.
/// </summary>
public record Keymap(ImmutableDictionary<KeyDownEventRecord, CommandRecord> Map)
{
    public Keymap() : this(ImmutableDictionary<KeyDownEventRecord, CommandRecord>.Empty)
    {
    }
}