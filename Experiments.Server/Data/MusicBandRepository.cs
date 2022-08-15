namespace Experiments.Server.Data;

public class MusicBandRepository : IMusicBandRepository
{
    private readonly Random _random = new();
    private readonly List<IMusicBand> _musicBands = new();

    private readonly MusicBand[] _seedDataAtoms = new []
    {
        new MusicBand("Crystal Castles", MusicBandKind.Electronic),
        new MusicBand("Porter Robinson", MusicBandKind.Electronic),
        new MusicBand("Sum 41", MusicBandKind.Other),
        new MusicBand("The Struts", MusicBandKind.Other),
        new MusicBand("Krewella", MusicBandKind.Electronic),
        new MusicBand("Paramore", MusicBandKind.Other),
        new MusicBand("Blink 182", MusicBandKind.Other),
        new MusicBand("Linkin Park", MusicBandKind.Other),
        new MusicBand("Tiësto", MusicBandKind.Electronic),
        new MusicBand("Basshunter", MusicBandKind.Electronic),
    };

    public MusicBandRepository(int seedCount)
    {
        for (int i = 0; i < seedCount; i++)
        {
            var seedAtom = _seedDataAtoms[_random.Next(0, _seedDataAtoms.Length - 1)];

            _musicBands.Add(seedAtom with
            {
                SeedIndex = i
            });
        }
    }

    public List<IMusicBand> GetMusicBands()
    {
        return _musicBands;
    }
}
