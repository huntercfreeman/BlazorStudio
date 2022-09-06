namespace TestingThings.Server.Music;

public record MusicBandKey(Guid Guid)
{ 
    public static MusicBandKey NewMusicBandKey()
    {
        return new(Guid.NewGuid());
    }
}