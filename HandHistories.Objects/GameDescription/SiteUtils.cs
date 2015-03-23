using System;
using System.Linq;

namespace HandHistories.Objects.GameDescription
{
    public static class SiteUtils
    {        
        public static string GetDisplaySiteName(SiteName siteName)
        {
            switch (siteName)
            {
                case SiteName.PokerStars:
                    return "Poker Stars";
                case SiteName.PartyPoker:
                    return "Party Poker";
                case SiteName.Pacific:
                    return "888";
                case SiteName.PokerStarsIt:
                    return "Poker Stars It";
                case SiteName.PokerStarsFr:
                    return "Poker Stars Fr";
                case SiteName.PokerStarsEs:
                    return "Poker Stars Es";
                case SiteName.IPoker:
                    return "iPoker";
                case SiteName.IPoker2:
                    return "iPoker - Low";
                default:
                    return siteName.ToString();
            }                        
        }

        public static SiteName ParseSiteName(string site)
        {
            if (string.IsNullOrEmpty(site))
            {
                return SiteName.Unknown;
            }

            switch (site.ToLower())
            {      
                case "all":
                    return SiteName.All;
                case "ftp":
                case "fulltilt":
                case "fulltiltpoker":
                    return SiteName.FullTilt;
                case "ps":
                case "pokerstars":
                case "stars":
                case "strs":
                    return SiteName.PokerStars;
                case "partypoker":
                case "party":
                case "pty":
                case "pp":
                    return SiteName.PartyPoker;
                case "ipoker1":
                case "ipoker-top":
                case "ipoker - top":
                case "ipoker":
                case "titanpoker":
                case "titan":
                case "poker770":
                case "expektpoker":
                    return SiteName.IPoker;
                case "absolute":
                case "cereus":
                case "ub":
                case "ultimatebet":
                    return SiteName.Cereus;
                case "ongame":
                case "on game":
                case "bestpoker":
                    return SiteName.OnGame;
                case "bodog":
                    return SiteName.Bodog;
                case "merge":
                case "carbon":
                case "carbonpoker":
                    return SiteName.Merge;
                case "entraction":
                    return SiteName.Entraction;
                case "microgaming":
                    return SiteName.MicroGaming;
                case "ladbrokes":
                    return SiteName.Ladbrokes;
                case "everest":
                    return SiteName.Everest;
                case "international":
                case "bossmedia":
                case "boss":
                case "paradise":
                    return SiteName.BossMedia;
                case "dollaropoker":
                    return SiteName.DollaroPoker;
                case "starsfr":
                case "pokerstarsfr":
                    return SiteName.PokerStarsFr;
                case "starsit":
                case "pokerstarsit":
                    return SiteName.PokerStarsIt;
                case "starses":
                case "pokerstarses":
                    return SiteName.PokerStarsEs;
                case "partyfr":
                case "partypokerfr":
                    return SiteName.PartyPokerFr;
                case "partyit":
                case "partypokerit":
                    return SiteName.PartyPokerIt;
                case "partynj":
                case "partypokernj":
                    return SiteName.PartyPokerNJ;
                case "partyes":
                case "partypokeres":
                    return SiteName.PartyPokerEs;
                case "ongameit":
                    return SiteName.OnGameIt;
                case "ongamefr":
                    return SiteName.OnGameFr;
                case "ipokerit":
                    return SiteName.IPokerIt;
                case "ipokerfr":
                    return SiteName.IPokerFr;
                case "ipoker2":
                case "ipoker - low":
                case "ipoker-low":
                case "betmost":
                    return SiteName.IPoker2;
                case "888":
                case "888poker":
                case "pacific":
                    return SiteName.Pacific;  
                case "winamax":
                case "winamaxfr":
                case "winamax.fr":
                    return SiteName.Winamax;
                default:
                    string match = Enum.GetNames(typeof (SiteName)).FirstOrDefault(s => s.ToLower().Equals(site.ToLower()));
                    return match == null ? SiteName.Unknown : (SiteName) Enum.Parse(typeof (SiteName), match,true);
            }
        }
    }
}
