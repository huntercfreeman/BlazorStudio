namespace BlazorStudio.ClassLib.TextEditor.IndexWrappers;

/// <summary>
/// Perhaps it is a good idea to use a horizontal
/// virtualization technique when rendering the UserInterface.
/// <br/>
/// Therefore <see cref="RectangularCoordinates"/> might have
/// <br/><br/>
/// a TopLeft: (RowIndex, ColumnIndex);
/// <br/>
/// and a BottomRight: (RowIndex, ColumnIndex);
/// <br/><br/>
/// For now I will only do vertical Virtualization
/// and ignore the <see cref="ColumnIndex"/>(s)
/// </summary>
public record RectangularCoordinates(
    (RowIndex RowIndex, ColumnIndex ColumnIndex) TopLeftCorner,
    (RowIndex RowIndex, ColumnIndex ColumnIndex) BottomRightCorner);
