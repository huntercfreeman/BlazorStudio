using Experiments.Server.Data;
using Microsoft.AspNetCore.Components;

namespace Experiments.Server.Components;

public partial class ListExample : ComponentBase
{
    [Inject]
    private IMusicBandRepository MusicBandRepository { get; set; } = null!;
}
