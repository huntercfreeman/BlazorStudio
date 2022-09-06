using System.Collections.Immutable;

namespace TestingThings.Server.Music;

public class MusicBandRepository : IMusicBandRepository
{
    private List<MusicBand> _musicBands = new();

    public ImmutableArray<MusicBand> GetMusicBands() => _musicBands.ToImmutableArray();

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
                    new MusicSong("Musician", "EDM", "https://youtu.be/q-74HTjRbuY"),,
                    new MusicSong("Mirror", "EDM", "https://youtu.be/PkiIPzG37vQ"),,
                    new MusicSong("Get your Wish", "EDM", "https://youtu.be/4SZEDBFPpgw"),
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Sefa",
                "EDM",
                MusicBandKey.NewMusicBandKey(), 
                new[]
                {
                    new MusicSong("Evil Activities - Nobody Said It Was Easy (Sefa Remix)", "EDM", "https://youtu.be/4rIW1Xz63Wc"),
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
                    new MusicSong("Europe - The Final Countdown (Azept Hardstyle Bootleg)", "EDM", "https://youtu.be/DsaHrjjz-hU"),
                    new MusicSong("Sia - Chandelier (Azept Hardstyle Bootleg)", "EDM", "https://youtu.be/41m9uJeD_nw")
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Re-vamp",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Lady Gaga & Bradley Cooper - Shallow (Re-vamp bootleg)", "EDM", "https://youtu.be/64wom1cMWxk")
                }.ToImmutableArray()));

        _musicBands.Add(
            new MusicBand(
                "Akidaraz",
                "EDM",
                MusicBandKey.NewMusicBandKey(),
                new[]
                {
                    new MusicSong("Avril Lavigne - Complicated (Akidaraz Bootleg)", "EDM", "https://youtu.be/wxg36yym3H4")
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
        
        _musicBands.Add(new MusicBand
        {
            BandName = "The Used",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong("Bulimic", "Alternative/Indie", "https://youtu.be/IY7dgz1dC_E"),
                new MusicSong("Noise and Kisses", "Alternative/Indie", "https://youtu.be/p4_60Zj7uaQ"),
                new MusicSong("On My Own", "Alternative/Indie", "https://youtu.be/jdPeR3jr6ek")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Radiohead",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong("How To Disappear Completely", "Alternative/Indie", "https://youtu.be/nZq_jeYsbTs"),
                new MusicSong("No Surprises", "Alternative/Indie", "https://youtu.be/u5CVsCnxyXg"),
                new MusicSong("Weird Fishes/Arpeggi", "Alternative/Indie", "https://youtu.be/pbEGdDOhIXg")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Paramore",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong("Brick By Boring Brick", "Alternative/Indie", "https://youtu.be/A63VwWz1ij0"),
                new MusicSong("The Only Exception", "Alternative/Indie", "https://youtu.be/-J7J_IWUhls"),
                new MusicSong("Emergency", "Alternative/Indie", "https://youtu.be/mgJ8BZi3vTA")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "My Chemical Romance",
            Genre = "Alternative Rock",
            Songs = new[] 
            {
                new MusicSong("Desert Song", "Alternative Rock", "https://youtu.be/NxQVElXpTVg"),
                new MusicSong("Early Sunsets Over Monroeville", "Alternative Rock", "https://youtu.be/fOLfblOB2e8")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Sum 41",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong("Pieces", "Alternative/Indie", "https://youtu.be/By7ctqcWxyM"),
                new MusicSong("With Me", "Alternative/Indie", "https://youtu.be/g8z-qP34-1Y")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "blink-182",
            Genre = "Pop Punk",
            Songs = new[] 
            {
                new MusicSong("Not Now", "Pop Punk", "https://youtu.be/HvcOuExmeJg"),
                new MusicSong("Down", "Pop Punk", "https://youtu.be/XrTZT49u0kM"),
                new MusicSong("Adam's Song", "Pop Punk", "https://youtu.be/2MRdtXWcgIw")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Linkin Park",
            Genre = "Alternative Rock",
            Songs = new[] 
            {
                new MusicSong("In The End", "Alternative Rock", "https://youtu.be/eVTXPUF4Oz4"),
                new MusicSong("Waiting For The End", "Alternative Rock", "https://youtu.be/5qF_qbaWt3Q")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "NERVO",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("R3hab & NERVO & Ummet Ozcan - Revolution", "EDM", "https://youtu.be/TWwIztjs29Y"),
                new MusicSong("We're All No One ft. Afrojack, Steve Aoki", "EDM", "https://youtu.be/pyeClGsiT4U")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Crystal Castles",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Kept", "EDM", "https://crystalcastles.bandcamp.com/track/kept"),
                new MusicSong("Untrust Us", "EDM", "https://youtu.be/tZu3EUVJ8-4")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Ashlee Simpson",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong("Pieces Of Me", "Pop", "https://youtu.be/WJCsyLUCSXI")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Vanessa Carlton",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong("A Thousand Miles", "Pop", "https://youtu.be/Cwkej79U3ek")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Carly Rae Jepsen",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong("Run Away With Me", "Pop", "https://youtu.be/TeccAtqd5K8")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Nelly Furtado",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong("I'm Like A Bird", "Pop", "https://youtu.be/roPQ_M3yJTA")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Counting Crows",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong("Mr. Jones", "Pop", "https://youtu.be/-oqAU5VxFWs")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "The Struts",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong("Could Have Been Me", "Alternative/Indie", "https://youtu.be/ARhk9K_mviE")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "MisterWives",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong("Our Own House", "Alternative/Indie", "https://youtu.be/Iets6iLm3QY"),
                new MusicSong("Reflections", "Alternative/Indie", "https://youtu.be/fBrOwiHO-5w")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "OneRepublic",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong("If I Lose Myself", "Pop", "https://youtu.be/TGx0rApSk6w")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Daughter",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong("Doing The Right Thing", "Alternative/Indie", "https://youtu.be/bU5F-DvGLkA"),
                new MusicSong("Run", "Alternative/Indie", "https://youtu.be/psiILfa-G1c"),
                new MusicSong("Youth", "Alternative/Indie", "https://youtu.be/VYC29lbq8SY"),
                new MusicSong("Switzerland", "Alternative/Indie", "https://youtu.be/BDm0qtQWA_o")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Jack Novak",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Driving Blind featuring Bright Lights", "EDM", "https://youtu.be/Ghjb_wU9-GY")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Hardwell",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("feat. Matthew Koma - Dare You", "EDM", "https://youtu.be/clRjbYa4UWQ")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Yall",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Together", "EDM", "https://youtu.be/iSn5bpEfuO8")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Borgeous",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Invincible", "EDM", "https://youtu.be/Yb5j4GheNTk")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Zedd",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Beautiful Now ft. Jon Bellion", "EDM", "https://youtu.be/n1a7o44WxNo")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Afrojack",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Ten Feet Tall ft. Wrabel", "EDM", "https://youtu.be/bltr_Dsk5EY")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "The Prophet",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Listen To Your Heart", "EDM", "https://youtu.be/wZEtCpIzU3E")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Johann Pachelbel",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Canon In D (Jatimatic Hardstyle Bootleg)", "EDM", "https://youtu.be/KBMO_4Nj4HQ")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Xillions",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Somebody Like Me (Mark With a K RMX)", "EDM", "https://youtu.be/b6NCaD9LjrI")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Hatsune Miku",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("Ievan Polkka (Theo Gobensen Hardstyle Remix)", "EDM", "https://youtu.be/f23SOgDxx6o")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Basshunter",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("DotA", "EDM", "https://youtu.be/qTsaS1Tm-Ic"),
                new MusicSong("All I Ever Wanted", "EDM", "https://youtu.be/P3CxhBIrBho"),
                new MusicSong("Now You're Gone", "EDM", "https://youtu.be/IgFwiCApH7E")
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Sandro Silva",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong("feat. Jack Miz - Let Go Tonight", "EDM", "https://youtu.be/yDJ5DjmSbQY")
            }.ToImmutableArray()
        });
    }
}