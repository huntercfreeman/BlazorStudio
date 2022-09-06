using Microsoft.AspNetCore.Components;
using TestingThings.Server.Music;

namespace TestingThings.Server.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private IMusicBandRepository MusicBandRepository { get; set; } = null!;

    private int _repeatTheDataCount;
    private DateTime _onInitializedDateTime;
    private DateTime _onAfterRenderFirstRenderDateTime;

    private TimeSpan TimeToFirstRender => _onAfterRenderFirstRenderDateTime.Subtract(_onInitializedDateTime);
    
    private int RepeatTheDataCount
    {
        get => _repeatTheDataCount;
        set
        {
            if (value < 0)
            {
                _repeatTheDataCount = 0;
            }
            else
            {
                _repeatTheDataCount = value;
            }

            MusicBandRepository.MutatePersistedRepeatTheDataCount(_repeatTheDataCount);
        }
    }

    protected override void OnInitialized()
    {
        _repeatTheDataCount = MusicBandRepository.PersistedRepeatTheDataCount;

        _onInitializedDateTime = DateTime.UtcNow;
        
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _onAfterRenderFirstRenderDateTime = DateTime.UtcNow;
            StateHasChanged();
        }
        
        base.OnAfterRender(firstRender);
    }
}