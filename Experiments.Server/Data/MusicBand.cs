namespace Experiments.Server.Data;

public record MusicBand(string Title, MusicBandKind Genre, int SeedIndex = -1) 
    : IMusicBand;
