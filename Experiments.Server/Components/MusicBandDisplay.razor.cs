using Experiments.Server.Data;
using Microsoft.AspNetCore.Components;

namespace Experiments.Server.Components;

public partial class MusicBandDisplay : ComponentBase
{
    [Parameter]
    public IMusicBand MusicBand { get; set; } = null!;
}
