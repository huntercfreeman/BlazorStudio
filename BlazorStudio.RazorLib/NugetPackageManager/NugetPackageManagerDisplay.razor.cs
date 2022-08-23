using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.NugetPackageManagerCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NugetPackageManager;

public partial class NugetPackageManagerDisplay : FluxorComponent
{
    [Inject]
    private IState<NugetPackageManagerState> NugetPackageManagerStateWrapper { get; set; } = null!;

    private SequenceKey? _previousFocusRequestedSequenceKey;
    private ElementReference? _nugetPackageManagerDisplayElementReference;
    
    protected override void OnInitialized()
    {
        NugetPackageManagerStateWrapper.StateChanged += NugetPackageManagerStateWrapperOnStateChanged;
        
        base.OnInitialized();
    }

    private async void NugetPackageManagerStateWrapperOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousFocusRequestedSequenceKey is null ||
            _previousFocusRequestedSequenceKey != NugetPackageManagerStateWrapper.Value.FocusRequestedSequenceKey)
        {
            if (_nugetPackageManagerDisplayElementReference is not null) 
                await _nugetPackageManagerDisplayElementReference.Value.FocusAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        NugetPackageManagerStateWrapper.StateChanged -= NugetPackageManagerStateWrapperOnStateChanged;

        base.Dispose(disposing);
    }
}