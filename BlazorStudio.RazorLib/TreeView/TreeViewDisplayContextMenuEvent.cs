using BlazorStudio.ClassLib.TreeView;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TreeView;

/// <summary>
/// If the context menu event occurred due to
/// a RightClick of the mouse then <see cref="CapturedMouseEventArgs"/>
/// will not be null, and will contain the
/// <see cref="MouseEventArgs"/>.
/// <br/><br/>
/// If the context menu event occurred due to a keyboard
/// input then the <see cref="CapturedMouseEventArgs"/> will be
/// null.
/// </summary>
public record TreeViewDisplayContextMenuEvent<TItem>(
    TreeViewDisplay<TItem> TreeViewDisplay,
    MouseEventArgs? CapturedMouseEventArgs);