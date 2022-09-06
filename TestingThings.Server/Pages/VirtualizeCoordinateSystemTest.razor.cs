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
    private VirtualizeCoordinateSystemScrollPosition? _virtualizeCoordinateSystemScrollPosition;

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
        _virtualizeCoordinateSystemScrollPosition = virtualizeCoordinateSystemScrollPosition;

        InvokeAsync(StateHasChanged);
        
        var totalWidth = 500;
        var totalHeight = 500;
        
        var contentWidth = 100;
        var contentHeight = 100;
        var contentLeftOffset = 100;
        var contentTopOffset = 100;

        var leftBoundaryDimension = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = contentLeftOffset,
            Height = totalHeight,
            Left = 0,
            Top = 0
        };
        
        var bottomBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = totalWidth,
            Height = totalHeight - (contentTopOffset + contentHeight),
            Left = 0,
            Top = contentTopOffset + contentHeight 
        };
        
        var topBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = contentWidth,
            Height = virtualizeCoordinateSystemScrollPosition.ScrollTop,
            Left = 0,
            Top = 0
        };
        
        var rightBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = totalWidth - (contentLeftOffset + contentWidth),
            Height = totalHeight,
            Left = contentLeftOffset + contentWidth,
            Top = 0
        } ;

        var itemsToRender = Array
            .Empty<VirtualizeCoordinateSystemEntry<MusicBand>>()
            .ToImmutableArray();
        
        return new VirtualizeCoordinateSystemResult<MusicBand>()
        {
            ItemsToRender = itemsToRender, 
            LeftBoundaryDimensions = leftBoundaryDimension,
            BottomBoundaryDimensions = bottomBoundaryDimensions,
            TopBoundaryDimensions = topBoundaryDimensions,
            RightBoundaryDimensions = rightBoundaryDimensions 
        };
    }
}