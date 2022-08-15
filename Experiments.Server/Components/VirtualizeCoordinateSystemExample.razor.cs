using Experiments.Server.Data;
using Microsoft.AspNetCore.Components;

namespace Experiments.Server.Components;

public partial class VirtualizeCoordinateSystemExample : ComponentBase
{
    [Inject]
    private IMusicBandRepository MusicBandRepository { get; set; } = null!;
}
