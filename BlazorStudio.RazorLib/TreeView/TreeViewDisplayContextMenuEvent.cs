using BlazorStudio.ClassLib.TreeView;
using BlazorStudio.RazorLib.JavaScriptObjects;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TreeView;

public record TreeViewDisplayContextMenuEvent<TItem>(
    TreeViewDisplay<TItem> TreeViewDisplay,
    ContextMenuFixedPosition ContextMenuFixedPosition)
{
    public string DefaultStyleForContextMenu => GetStyleForContextMenu();

    /// <summary>
    /// $"position: fixed; left: {leftInPixels}px; top: {topInPixels}px;";
    /// <br/><br/>
    /// If OccurredDueToMouseEvent is true on <see cref="ContextMenuFixedPosition"/>
    /// then the values for left and top respectively match clientX and clientY of the
    /// mouse event.
    /// <br/><br/>
    /// If OccurredDueToMouseEvent is true on <see cref="ContextMenuFixedPosition"/>
    /// then the values for left and top are what the JavaScript function
    /// window.BlazorStudio.getContextMenuFixedPositionRelativeToElement() returns
    /// but with the left increased by how much offset there is on the tree view display
    /// due to its depth.
    /// </summary>
    private string GetStyleForContextMenu()
    {
        var leftInPixels = ContextMenuFixedPosition.LeftPositionInPixels;

        if (!ContextMenuFixedPosition.OccurredDueToMouseEvent)
            leftInPixels += TreeViewDisplay.OffsetLeft;
        
        var topInPixels = ContextMenuFixedPosition.TopPositionInPixels;
        
        return 
            $"position: fixed; left: {leftInPixels}px; top: {topInPixels}px;";
    }
}