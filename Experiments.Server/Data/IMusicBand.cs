namespace Experiments.Server.Data;

public interface IMusicBand
{
    public string Title { get; }
    public MusicBandKind Genre { get; }
    public int SeedIndex { get; }
}
