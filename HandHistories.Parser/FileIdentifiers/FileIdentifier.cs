using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.FileIdentifiers;

namespace HandHistories.Parser.FileIdentifiers
{
    public static class FileIdentifier
    {
        static List<IFileIdentifier> Identifiers = InitIdentifiers();

        private static List<IFileIdentifier> InitIdentifiers()
        {
            List<IFileIdentifier> ids = new List<IFileIdentifier>();

            ids.Add(new Poker888.Poker888FileIdentifier());
            ids.Add(new BossMedia.BossMediaFileIdentifier());
            ids.Add(new FullTiltPoker.FullTiltPokerFileIdentifier());
            ids.Add(new IPoker.IPokerFileIdentifier());
            ids.Add(new MicroGaming.MicroGamingFileIdentifier());
            ids.Add(new OnGame.OnGameFileIdentifier());
            ids.Add(new PartyPoker.PartyPokerFileIdentifier());
            ids.Add(new PokerStars.PokerStarsFileIdentifier());
            ids.Add(new Winamax.WinamaxFileIdentifier());
            ids.Add(new WinningPoker.WinningPokerV1FileIdentifier());
            ids.Add(new WinningPoker.WinningPokerV2FileIdentifier());
            ids.Add(new IGT.IGTFileIdentifier());
            ids.Add(new AsianPokerClubs.AsianPokerClubsFileIdentifier());

            return ids;
        }

        public static SiteName IdentifyHand(string text)
        {
            var match = Identifiers.FirstOrDefault(i => i.Match(text));
            if (match != null)
            {
                return match.Site;
            }

            return SiteName.Unknown;
        }
    }
}
