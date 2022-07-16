using BlazorStudio.ClassLib.Errors;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Errors;

public partial class RichErrorDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public RichErrorModel RichErrorModel { get; set; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (RichErrorModel.IsResolved is not null && RichErrorModel.IsResolved() &&
            RichErrorModel.OnIsResolvedAction is not null)
            RichErrorModel.OnIsResolvedAction(RichErrorModel);

        base.OnAfterRender(firstRender);
    }
}