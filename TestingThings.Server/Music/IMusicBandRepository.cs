using System.Collections.Immutable;

namespace TestingThings.Server.Music;

public interface IMusicBandRepository
{
    public int PersistedRepeatTheDataCount { get; }

    public event EventHandler OnPersistedRepeatTheDataCountChanged;
    
    public void MutatePersistedRepeatTheDataCount(int repeatTheDataCount);
    public ImmutableArray<MusicBand> GetMusicBands(int repeatTheDataCount = 0);
    public void AddMusicBand(MusicBand musicBand);
    public void RemoveMusicBand(MusicBandKey musicBandKey);
}