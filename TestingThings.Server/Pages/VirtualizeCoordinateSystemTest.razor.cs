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
    private int _totalWidth = 5000;
    private int _totalHeight = 5000;
    private int _scrollViewportWidth = 800;
    private int _scrollViewportHeight = 600;
    private VirtualizeCoordinateSystemScrollPosition? _virtualizeCoordinateSystemScrollPosition;
    private VirtualizeCoordinateSystemBoundaryDimensions _leftBoundaryDimension = new();
    private VirtualizeCoordinateSystemBoundaryDimensions _bottomBoundaryDimensions = new();
    private VirtualizeCoordinateSystemBoundaryDimensions _topBoundaryDimensions = new();
    private VirtualizeCoordinateSystemBoundaryDimensions _rightBoundaryDimensions = new();
    private ImmutableArray<VirtualizeCoordinateSystemEntry<MusicBand>> _itemsToRender = ImmutableArray<VirtualizeCoordinateSystemEntry<MusicBand>>.Empty;

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

        _totalWidth = 5000;
        _totalHeight = 5000;

        var contentWidth = 0;
        var contentHeight = 0;
        var contentLeftOffset = virtualizeCoordinateSystemScrollPosition.ScrollLeft;
        var contentTopOffset = virtualizeCoordinateSystemScrollPosition.ScrollTop;

        // Validate minimum dimensions
        {
            contentWidth = contentWidth > _scrollViewportWidth
                ? contentWidth
                : _scrollViewportWidth;
            
            contentHeight = contentHeight > _scrollViewportHeight
                ? contentHeight
                : _scrollViewportHeight;
        }
        
        _leftBoundaryDimension = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = contentLeftOffset,
            Height = _totalHeight,
            Left = 0,
            Top = 0
        };
        
        _bottomBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = _totalWidth,
            Height = _totalHeight - (contentTopOffset + contentHeight),
            Left = 0,
            Top = contentTopOffset + contentHeight 
        };
        
        _topBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = contentWidth,
            Height = virtualizeCoordinateSystemScrollPosition.ScrollTop,
            Left = 0,
            Top = 0
        };
        
        _rightBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = _totalWidth - (contentLeftOffset + contentWidth),
            Height = _totalHeight,
            Left = contentLeftOffset + contentWidth,
            Top = 0
        };

        _itemsToRender = Array
            .Empty<VirtualizeCoordinateSystemEntry<MusicBand>>()
            .ToImmutableArray();
        
        return new VirtualizeCoordinateSystemResult<MusicBand>()
        {
            ItemsToRender = _itemsToRender, 
            LeftBoundaryDimensions = _leftBoundaryDimension,
            BottomBoundaryDimensions = _bottomBoundaryDimensions,
            TopBoundaryDimensions = _topBoundaryDimensions,
            RightBoundaryDimensions = _rightBoundaryDimensions 
        };
    }
}
