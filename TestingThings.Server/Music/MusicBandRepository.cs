using System.Collections.Immutable;

namespace TestingThings.Server.Music;

public class MusicBandRepository : IMusicBandRepository
{
    private List<MusicBand> _musicBands = new();
    
    public MusicBandRepository()
    {
        _musicBands.Add(
            new MusicBand(
                "Krewella",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Alive", "EDM", "https://youtu.be/J-gYJBsln-w"),
                    new MusicSong("Greenlights", "EDM", "https://youtu.be/s2dgChEZA4M"),
                    new MusicSong("Killin' It", "EDM", "https://youtu.be/EZ9-1WD-rBA")
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Porter Robinson",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Musician", "EDM", "https://youtu.be/q-74HTjRbuY"),
                    new MusicSong("Mirror", "EDM", "https://youtu.be/PkiIPzG37vQ"),
                    new MusicSong("Get your Wish", "EDM", "https://youtu.be/4SZEDBFPpgw"),
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Sefa",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Evil Activities - Nobody Said It Was Easy (Sefa Remix)", "EDM",
                        "https://youtu.be/4rIW1Xz63Wc"),
                    new MusicSong("Crawling", "EDM", "https://youtu.be/mJEaNIK8vj4"),
                    new MusicSong("Schopenhauer", "EDM", "https://youtu.be/82EjuxJjtRU")
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Azept",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Europe - The Final Countdown (Azept Hardstyle Bootleg)", "EDM",
                        "https://youtu.be/DsaHrjjz-hU"),
                    new MusicSong("Sia - Chandelier (Azept Hardstyle Bootleg)", "EDM", "https://youtu.be/41m9uJeD_nw")
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Re-vamp",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Lady Gaga & Bradley Cooper - Shallow (Re-vamp bootleg)", "EDM",
                        "https://youtu.be/64wom1cMWxk")
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Akidaraz",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Avril Lavigne - Complicated (Akidaraz Bootleg)", "EDM",
                        "https://youtu.be/wxg36yym3H4")
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Avril Lavigne",
                "Pop",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("When You're Gone", "Pop", "https://youtu.be/0G3_kG5FFfQ"),
                    new MusicSong("I'm With You", "Pop", "https://youtu.be/dGR65RWwzg8"),
                    new MusicSong("Sk8er Boi", "Pop", "https://youtu.be/TIy3n2b7V9k")
                }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("The Used", "Alternative/Indie", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Bulimic", "Alternative/Indie", "https://youtu.be/IY7dgz1dC_E"),
            new MusicSong("Noise and Kisses", "Alternative/Indie", "https://youtu.be/p4_60Zj7uaQ"),
            new MusicSong("On My Own", "Alternative/Indie", "https://youtu.be/jdPeR3jr6ek")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Radiohead", "Alternative/Indie", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("How To Disappear Completely", "Alternative/Indie", "https://youtu.be/nZq_jeYsbTs"),
            new MusicSong("No Surprises", "Alternative/Indie", "https://youtu.be/u5CVsCnxyXg"),
            new MusicSong("Weird Fishes/Arpeggi", "Alternative/Indie", "https://youtu.be/pbEGdDOhIXg")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Paramore", "Alternative/Indie", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Brick By Boring Brick", "Alternative/Indie", "https://youtu.be/A63VwWz1ij0"),
            new MusicSong("The Only Exception", "Alternative/Indie", "https://youtu.be/-J7J_IWUhls"),
            new MusicSong("Emergency", "Alternative/Indie", "https://youtu.be/mgJ8BZi3vTA")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("My Chemical Romance", "Alternative Rock", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Desert Song", "Alternative Rock", "https://youtu.be/NxQVElXpTVg"),
            new MusicSong("Early Sunsets Over Monroeville", "Alternative Rock", "https://youtu.be/fOLfblOB2e8")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Sum 41", "Alternative/Indie", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Pieces", "Alternative/Indie", "https://youtu.be/By7ctqcWxyM"),
            new MusicSong("With Me", "Alternative/Indie", "https://youtu.be/g8z-qP34-1Y")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("blink-182", "Pop Punk", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Not Now", "Pop Punk", "https://youtu.be/HvcOuExmeJg"),
            new MusicSong("Down", "Pop Punk", "https://youtu.be/XrTZT49u0kM"),
            new MusicSong("Adam's Song", "Pop Punk", "https://youtu.be/2MRdtXWcgIw")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Linkin Park", "Alternative Rock", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("In The End", "Alternative Rock", "https://youtu.be/eVTXPUF4Oz4"),
            new MusicSong("Waiting For The End", "Alternative Rock", "https://youtu.be/5qF_qbaWt3Q")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("NERVO", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("R3hab & NERVO & Ummet Ozcan - Revolution", "EDM", "https://youtu.be/TWwIztjs29Y"),
            new MusicSong("We're All No One ft. Afrojack, Steve Aoki", "EDM", "https://youtu.be/pyeClGsiT4U")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Crystal Castles", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Kept", "EDM", "https://crystalcastles.bandcamp.com/track/kept"),
            new MusicSong("Untrust Us", "EDM", "https://youtu.be/tZu3EUVJ8-4")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Ashlee Simpson", "Pop", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Pieces Of Me", "Pop", "https://youtu.be/WJCsyLUCSXI")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Vanessa Carlton", "Pop", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("A Thousand Miles", "Pop", "https://youtu.be/Cwkej79U3ek")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Carly Rae Jepsen", "Pop", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Run Away With Me", "Pop", "https://youtu.be/TeccAtqd5K8")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Nelly Furtado", "Pop", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("I'm Like A Bird", "Pop", "https://youtu.be/roPQ_M3yJTA")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Counting Crows", "Pop", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Mr. Jones", "Pop", "https://youtu.be/-oqAU5VxFWs")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("The Struts", "Alternative/Indie", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Could Have Been Me", "Alternative/Indie", "https://youtu.be/ARhk9K_mviE")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("MisterWives", "Alternative/Indie", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Our Own House", "Alternative/Indie", "https://youtu.be/Iets6iLm3QY"),
            new MusicSong("Reflections", "Alternative/Indie", "https://youtu.be/fBrOwiHO-5w")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("OneRepublic", "Pop", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("If I Lose Myself", "Pop", "https://youtu.be/TGx0rApSk6w")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Daughter", "Alternative/Indie", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Doing The Right Thing", "Alternative/Indie", "https://youtu.be/bU5F-DvGLkA"),
            new MusicSong("Run", "Alternative/Indie", "https://youtu.be/psiILfa-G1c"),
            new MusicSong("Youth", "Alternative/Indie", "https://youtu.be/VYC29lbq8SY"),
            new MusicSong("Switzerland", "Alternative/Indie", "https://youtu.be/BDm0qtQWA_o")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Jack Novak", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Driving Blind featuring Bright Lights", "EDM", "https://youtu.be/Ghjb_wU9-GY")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Hardwell", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("feat. Matthew Koma - Dare You", "EDM", "https://youtu.be/clRjbYa4UWQ")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Yall", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Together", "EDM", "https://youtu.be/iSn5bpEfuO8")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Borgeous", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Invincible", "EDM", "https://youtu.be/Yb5j4GheNTk")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Zedd", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Beautiful Now ft. Jon Bellion", "EDM", "https://youtu.be/n1a7o44WxNo")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Afrojack", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Ten Feet Tall ft. Wrabel", "EDM", "https://youtu.be/bltr_Dsk5EY")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("The Prophet", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Listen To Your Heart", "EDM", "https://youtu.be/wZEtCpIzU3E")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Johann Pachelbel", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Canon In D (Jatimatic Hardstyle Bootleg)", "EDM", "https://youtu.be/KBMO_4Nj4HQ")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Xillions", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Somebody Like Me (Mark With a K RMX)", "EDM", "https://youtu.be/b6NCaD9LjrI")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Hatsune Miku", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("Ievan Polkka (Theo Gobensen Hardstyle Remix)", "EDM", "https://youtu.be/f23SOgDxx6o")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Basshunter", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("DotA", "EDM", "https://youtu.be/qTsaS1Tm-Ic"),
            new MusicSong("All I Ever Wanted", "EDM", "https://youtu.be/P3CxhBIrBho"),
            new MusicSong("Now You're Gone", "EDM", "https://youtu.be/IgFwiCApH7E")
        }.ToImmutableArray()));

        _musicBands.Add(new MusicBand("Sandro Silva", "EDM", MusicBandKey.NewMusicBandKey(), new[]
        {
            new MusicSong("feat. Jack Miz - Let Go Tonight", "EDM", "https://youtu.be/yDJ5DjmSbQY")
        }.ToImmutableArray()));
    }

    public int PersistedRepeatTheDataCount { get; private set; } = 1000;
    
    public event EventHandler OnPersistedRepeatTheDataCountChanged;

    public void MutatePersistedRepeatTheDataCount(int repeatTheDataCount)
    {
        PersistedRepeatTheDataCount = repeatTheDataCount;

        OnPersistedRepeatTheDataCountChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public ImmutableArray<MusicBand> GetMusicBands(int repeatTheDataCount = 0)
    {
        var result = new List<MusicBand>();

        for (int i = -1; i < repeatTheDataCount; i++)
        {
            result.AddRange(_musicBands);
        }
        
        return result.ToImmutableArray();
    }

    public void AddMusicBand(MusicBand musicBand)
    {
        throw new NotImplementedException();
    }

    public void RemoveMusicBand(MusicBandKey musicBandKey)
    {
        throw new NotImplementedException();
    }
}