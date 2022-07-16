using BlazorStudio.ClassLib.Errors;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Errors;

public partial class RichErrorListDisplay : ComponentBase
{
    [Parameter]
    public List<RichErrorModel> BlazorUiErrorModels { get; set; } = null!;

    public async Task RerenderAsync()
    {
        foreach (var error in BlazorUiErrorModels)
        {
            if (error.IsResolved is not null && error.IsResolved() &&
                error.OnIsResolvedAction is not null)
                error.OnIsResolvedAction(error);
        }

        await InvokeAsync(StateHasChanged);
    }
}