using Microsoft.AspNetCore.Components;
using TestingThings.Server.Music;

namespace TestingThings.Server.Shared;

public partial class NavMenu : ComponentBase, IDisposable
{
    [Inject]
    private IMusicBandRepository MusicBandRepository { get; set; } = null!;

    protected override void OnInitialized()
    {
        MusicBandRepository.OnPersistedRepeatTheDataCountChanged += MusicBandRepositoryOnOnPersistedRepeatTheDataCountChanged;
        
        base.OnInitialized();
    }

    private async void MusicBandRepositoryOnOnPersistedRepeatTheDataCountChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        MusicBandRepository.OnPersistedRepeatTheDataCountChanged -= MusicBandRepositoryOnOnPersistedRepeatTheDataCountChanged;
    }
}