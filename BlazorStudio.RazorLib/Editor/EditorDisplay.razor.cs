using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : ComponentBase
{
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;
}