using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.MenuCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuDisplay : ComponentBase
{
    [Parameter]
    public IEnumerable<MenuOptionRecord> MenuOptionRecords { get; set; } = null!;
    [Parameter]
    public bool ShouldCategorizeByMenuOptionKind { get; set; }

    private MenuOptionRecord[] _cachedMenuOptionRecords = Array.Empty<MenuOptionRecord>();

    protected override void OnInitialized()
    {
        ReloadParameters();

        base.OnInitialized();
    }

    private void ReloadParameters()
    {
        _cachedMenuOptionRecords = MenuOptionRecords
            .ToArray();
    }
    
    public async Task ReloadParametersAsync()
    {
        ReloadParameters();

        await InvokeAsync(StateHasChanged);
    }
}