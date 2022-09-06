using Microsoft.AspNetCore.Components;

namespace TestingThings.Server.Music;

public partial class MusicBandDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MusicBand MusicBand { get; set; } = null!;
}