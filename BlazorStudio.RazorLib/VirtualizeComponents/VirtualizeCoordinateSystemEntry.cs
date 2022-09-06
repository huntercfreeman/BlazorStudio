namespace BlazorStudio.RazorLib.VirtualizeComponents;

public class VirtualizeCoordinateSystemEntry<T>
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double Left { get; set; }
    public double Top { get; set; }
    public int Index { get; set; }
    public T Item { get; set; }
}