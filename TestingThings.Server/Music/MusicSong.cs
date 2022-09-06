namespace TestingThings.Server.Music;

public class MusicSong
{
    public MusicSong(string songName, string genre, string urlToSong)
    {
        SongName = songName;
        Genre = genre;
        UrlToSong = urlToSong;
    }
    
    public string SongName { get; }
    /// <summary>
    /// Bands can have a band level general genre that
    /// describes them and a per song genre. 
    /// </summary>
    public string Genre { get; }
    public string UrlToSong { get; }
}