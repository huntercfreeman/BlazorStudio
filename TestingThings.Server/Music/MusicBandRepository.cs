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
                new[]
                {
                    new MusicSong("Musician", "EDM", "https://youtu.be/q-74HTjRbuY"),,
                    new MusicSong("Mirror", "EDM", "https://youtu.be/PkiIPzG37vQ"),,
                    new MusicSong("Get your Wish", "EDM", "https://youtu.be/4SZEDBFPpgw"),
                }.ToImmutableArray()));
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Sefa",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Evil Activities - Nobody Said It Was Easy (Sefa Remix)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/4rIW1Xz63Wc";
                },
                new MusicSong
                {
                    SongName = "Crawling";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/mJEaNIK8vj4";
                },
                new MusicSong
                {
                    SongName = "Schopenhauer";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/82EjuxJjtRU";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Azept",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Europe - The Final Countdown (Azept Hardstyle Bootleg)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/DsaHrjjz-hU";
                },
                new MusicSong
                {
                    SongName = "Sia - Chandelier (Azept Hardstyle Bootleg)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/41m9uJeD_nw";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Re-vamp",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Lady Gaga & Bradley Cooper - Shallow (Re-vamp bootleg)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/64wom1cMWxk";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Akidaraz",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Avril Lavigne - Complicated (Akidaraz Bootleg)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/wxg36yym3H4";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Avril Lavigne",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "When You're Gone";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/0G3_kG5FFfQ";
                },
                new MusicSong
                {
                    SongName = "I'm With You";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/dGR65RWwzg8";
                },
                new MusicSong
                {
                    SongName = "Sk8er Boi";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/TIy3n2b7V9k";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "The Used",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Bulimic";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/IY7dgz1dC_E";
                },
                new MusicSong
                {
                    SongName = "Noise and Kisses";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/p4_60Zj7uaQ";
                },
                new MusicSong
                {
                    SongName = "On My Own";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/jdPeR3jr6ek";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Radiohead",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "How To Disappear Completely";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/nZq_jeYsbTs";
                },
                new MusicSong
                {
                    SongName = "No Surprises";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/u5CVsCnxyXg";
                },
                new MusicSong
                {
                    SongName = "Weird Fishes/Arpeggi";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/pbEGdDOhIXg";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Paramore",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Brick By Boring Brick";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/A63VwWz1ij0";
                },
                new MusicSong
                {
                    SongName = "The Only Exception";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/-J7J_IWUhls";
                },
                new MusicSong
                {
                    SongName = "Emergency";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/mgJ8BZi3vTA";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "My Chemical Romance",
            Genre = "Alternative Rock",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Desert Song";
                    Genre = "Alternative Rock";
                    UrlToSong = "https://youtu.be/NxQVElXpTVg";
                },
                new MusicSong
                {
                    SongName = "Early Sunsets Over Monroeville";
                    Genre = "Alternative Rock";
                    UrlToSong = "https://youtu.be/fOLfblOB2e8";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Sum 41",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Pieces";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/By7ctqcWxyM";
                },
                new MusicSong
                {
                    SongName = "With Me";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/g8z-qP34-1Y";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "blink-182",
            Genre = "Pop Punk",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Not Now";
                    Genre = "Pop Punk";
                    UrlToSong = "https://youtu.be/HvcOuExmeJg";
                },
                new MusicSong
                {
                    SongName = "Down";
                    Genre = "Pop Punk";
                    UrlToSong = "https://youtu.be/XrTZT49u0kM";
                },
                new MusicSong
                {
                    SongName = "Adam's Song";
                    Genre = "Pop Punk";
                    UrlToSong = "https://youtu.be/2MRdtXWcgIw";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Linkin Park",
            Genre = "Alternative Rock",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "In The End";
                    Genre = "Alternative Rock";
                    UrlToSong = "https://youtu.be/eVTXPUF4Oz4";
                },
                new MusicSong
                {
                    SongName = "Waiting For The End";
                    Genre = "Alternative Rock";
                    UrlToSong = "https://youtu.be/5qF_qbaWt3Q";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "NERVO",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "R3hab & NERVO & Ummet Ozcan - Revolution";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/TWwIztjs29Y";
                },
                new MusicSong
                {
                    SongName = "We're All No One ft. Afrojack, Steve Aoki";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/pyeClGsiT4U";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Crystal Castles",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Kept";
                    Genre = "EDM";
                    UrlToSong = "https://crystalcastles.bandcamp.com/track/kept";
                },
                new MusicSong
                {
                    SongName = "Untrust Us";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/tZu3EUVJ8-4";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Ashlee Simpson",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Pieces Of Me";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/WJCsyLUCSXI";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Vanessa Carlton",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "A Thousand Miles";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/Cwkej79U3ek";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Carly Rae Jepsen",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Run Away With Me";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/TeccAtqd5K8";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Nelly Furtado",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "I'm Like A Bird";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/roPQ_M3yJTA";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Counting Crows",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Mr. Jones";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/-oqAU5VxFWs";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "The Struts",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Could Have Been Me";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/ARhk9K_mviE";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "MisterWives",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Our Own House";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/Iets6iLm3QY";
                },
                new MusicSong
                {
                    SongName = "Reflections";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/fBrOwiHO-5w";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "OneRepublic",
            Genre = "Pop",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "If I Lose Myself";
                    Genre = "Pop";
                    UrlToSong = "https://youtu.be/TGx0rApSk6w";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Daughter",
            Genre = "Alternative/Indie",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Doing The Right Thing";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/bU5F-DvGLkA";
                },
                new MusicSong
                {
                    SongName = "Run";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/psiILfa-G1c";
                },
                new MusicSong
                {
                    SongName = "Youth";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/VYC29lbq8SY";
                },
                new MusicSong
                {
                    SongName = "Switzerland";
                    Genre = "Alternative/Indie";
                    UrlToSong = "https://youtu.be/BDm0qtQWA_o";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Jack Novak",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Driving Blind featuring Bright Lights";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/Ghjb_wU9-GY";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Hardwell",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "feat. Matthew Koma - Dare You";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/clRjbYa4UWQ";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Yall",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Together";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/iSn5bpEfuO8";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Borgeous",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Invincible";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/Yb5j4GheNTk";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Zedd",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Beautiful Now ft. Jon Bellion";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/n1a7o44WxNo";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Afrojack",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Ten Feet Tall ft. Wrabel";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/bltr_Dsk5EY";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "The Prophet",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Listen To Your Heart";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/wZEtCpIzU3E";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Johann Pachelbel",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Canon In D (Jatimatic Hardstyle Bootleg)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/KBMO_4Nj4HQ";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Xillions",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Somebody Like Me (Mark With a K RMX)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/b6NCaD9LjrI";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Hatsune Miku",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "Ievan Polkka (Theo Gobensen Hardstyle Remix)";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/f23SOgDxx6o";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Basshunter",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "DotA";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/qTsaS1Tm-Ic";
                },
                new MusicSong
                {
                    SongName = "All I Ever Wanted";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/P3CxhBIrBho";
                },
                new MusicSong
                {
                    SongName = "Now You're Gone";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/IgFwiCApH7E";
                }
            }.ToImmutableArray()
        });
        
        _musicBands.Add(new MusicBand
        {
            BandName = "Sandro Silva",
            Genre = "EDM",
            Songs = new[] 
            {
                new MusicSong
                {
                    SongName = "feat. Jack Miz - Let Go Tonight";
                    Genre = "EDM";
                    UrlToSong = "https://youtu.be/yDJ5DjmSbQY";
                }
            }.ToImmutableArray()
        });
    }
}