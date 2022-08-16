namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

/// <summary>
/// It can get confusing to have <see cref="VirtualizeBoundaryKind.Top"/>
/// and then refer to the css style 'top: 0px'.
/// <br/>--<br/>
/// So I refer to the css style 'top' as <see cref="VirtualizeBoundary.OffsetFromTopInPixels"/>
/// <br/>--<br/>
/// I considered using enums: 'North', and 'South' but I am ambivalent.
/// </summary>
public enum VirtualizeBoundaryKind
{
    Top,
    Bottom
}