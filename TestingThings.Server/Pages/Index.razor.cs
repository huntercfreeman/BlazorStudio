using Microsoft.AspNetCore.Components;
using TestingThings.Server.Music;

namespace TestingThings.Server.Pages;

public partial class Index : ComponentBase
{
    [Inject]
    private IMusicBandRepository MusicBandRepository { get; set; } = null!;
}