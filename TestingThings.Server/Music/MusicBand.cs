using System.Collections.Immutable;

namespace TestingThings.Server.Music;

public class MusicBand
{
    public MusicBand(string bandName, string genre, MusicBandKey musicBandKey, ImmutableArray<MusicSong> musicSongs)
    {
        BandName = bandName;
        Genre = genre;
        MusicBandKey = musicBandKey;
        MusicSongs = musicSongs;
    }
    
    public string BandName { get; }
    /// <summary>
    /// Bands can have a band level general genre that
    /// describes them and a per song genre. 
    /// </summary>
    public string Genre { get; }
    public MusicBandKey MusicBandKey { get; }
    public ImmutableArray<MusicSong> MusicSongs { get; }
}