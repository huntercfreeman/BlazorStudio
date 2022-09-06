using System.Collections.Immutable;
using BlazorStudio.RazorLib.VirtualizeComponents;
using Microsoft.AspNetCore.Components;
using TestingThings.Server.Music;

namespace TestingThings.Server.Pages;

public partial class VirtualizeCoordinateSystemTest : ComponentBase
{
    [Inject]
    private IMusicBandRepository MusicBandRepository { get; set; } = null!;

    private int _repeatTheDataCount;
    private DateTime _onInitializedDateTime;
    private DateTime _onAfterRenderFirstRenderDateTime;
    private int _totalWidth = 500;
    private int _totalHeight = 500;

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

    private VirtualizeCoordinateSystemResult<MusicBand> ItemsProviderFunc(
        VirtualizeCoordinateSystemScrollPosition virtualizeCoordinateSystemScrollPosition)
    {
        var totalWidth = 500;
        var totalHeight = 500;
        
        var contentWidth = 100;
        var contentHeight = 100;
        var contentLeftOffset = 100;
        var contentTopOffset = 100;
        
        return new()
        {
            ItemsToRender = Array.Empty<VirtualizeCoordinateSystemEntry<MusicBand>>().ToImmutableArray(),
            LeftBoundaryDimensions = new()
            {
                Width = contentLeftOffset,
                Height = totalHeight,
                Left = 0,
                Top = 0
            },
            BottomBoundaryDimensions = 
            {
                Width = totalWidth,
                Height = totalHeight - (contentTopOffset + contentHeight),
                Left = 0,
                Top = contentTopOffset + contentHeight 
            },
            TopBoundaryDimensions = 
            {
                Width = contentWidth,
                Height = virtualizeCoordinateSystemScrollPosition.ScrollTop,
                Left = 0,
                Top = 0
            },
            RightBoundaryDimensions = 
            {
                Width = totalWidth - (contentLeftOffset + contentWidth),
                Height = totalHeight,
                Left = contentLeftOffset + contentWidth,
                Top = 0
            } 
        };
    }
}