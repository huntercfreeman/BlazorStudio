namespace BlazorStudio.ClassLib.Contexts;

/// <summary>
///     ContextBoundary Blazor component is a Context Provider.
///     <br />--<br />
///     It works by using the Blazor CascadingValue component.
///     <br />--<br />
///     Each ContextBoundary CascadingValues its corresponding <see cref="ContextRecord" />
///     <br />--<br />
///     When focus is set to a Blazor component all the ContextBoundarys "bubble up" their
///     CascadingValues into a list of <see cref="ContextRecord" />.
///     <br />--<br />
///     The closest ancestor <see cref="ContextRecord" /> is first to attempt to handle the keyboard input
/// </summary>
public record ContextBoundaryKey(Guid Guid)
{
    public static ContextBoundaryKey NewContextBoundaryKey()
    {
        return new ContextBoundaryKey(Guid.NewGuid());
    }
}