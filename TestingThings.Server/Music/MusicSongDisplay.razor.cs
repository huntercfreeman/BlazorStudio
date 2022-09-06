using Microsoft.AspNetCore.Components;

namespace TestingThings.Server.Music;

public partial class MusicSongDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MusicSong MusicSong { get; set; } = null!;
}