using System.Collections.Immutable;

namespace TestingThings.Server.Music;

public interface IMusicBandRepository
{
    public ImmutableArray<MusicBand> GetMusicBands();
    public void AddMusicBand(MusicBand musicBand);
    public void RemoveMusicBand(MusicBandKey musicBandKey);
}