using System;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Compression;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser.Entraction;
using HandHistories.Parser.Parsers.FastParser.FullTiltPoker;
using HandHistories.Parser.Parsers.FastParser.IPoker;
using HandHistories.Parser.Parsers.FastParser.Merge;
using HandHistories.Parser.Parsers.FastParser.MicroGaming;
using HandHistories.Parser.Parsers.FastParser.OnGame;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Parser.Parsers.FastParser.Winamax;
using HandHistories.Parser.Parsers.FastParser._888;
using HandHistories.Parser.Parsers.RegexParser.PartyPoker;
using HandHistories.Parser.Parsers.FastParser.Winning;

namespace HandHistories.Parser.Parsers.Factory
{
    public class HandHistoryParserFactoryImpl : IHandHistoryParserFactory
    {
        public HandHistoryParserFactoryImpl()
        {
       
        }

        public IHandHistoryParser GetFullHandHistoryParser(SiteName siteName)
        {
            switch (siteName)
            {
                case SiteName.PartyPoker:
                    return new PartyHandHistoryRegexParserImpl();
                case SiteName.PokerStars:
                case SiteName.PokerStarsFr:
                case SiteName.PokerStarsIt:
                case SiteName.PokerStarsEs:
                    return new PokerStarsFastParserImpl(siteName);
                case SiteName.Merge:
                    return new MergeFastParserImpl();
                case SiteName.IPoker:
                    return new IPokerFastParserImpl();
                case SiteName.IPoker2:
                    return new IPokerFastParserImpl(true);
                case SiteName.OnGame:
                    return new OnGameFastParserImpl();
                case SiteName.OnGameFr:
                    return new OnGameFastParserImpl(SiteName.OnGameFr);
                case SiteName.OnGameIt:
                    return new OnGameFastParserImpl(SiteName.OnGameIt);                    
                case SiteName.Pacific:
                    return new Poker888FastParserImpl();
                case SiteName.Entraction:
                    return new EntractionFastParserImpl();
                case SiteName.FullTilt:
                    return new FullTiltPokerFastParserImpl();
                case SiteName.MicroGaming:
                    return new MicroGamingFastParserImpl();
                case SiteName.Winamax:
                    return new WinamaxFastParserImpl();
                case SiteName.WinningPoker:
                    return new WinningPokerNetworkFastParserImpl();
                default:
                    throw new NotImplementedException("GetHandHistorySummaryParser: No full regex parser for " + siteName);
            }
        }

        public IHandHistorySummaryParser GetHandHistorySummaryParser(SiteName siteName)
        {
            switch (siteName)
            {
                case SiteName.PartyPoker:
                    return new PartyHandHistoryRegexParserImpl();
                case SiteName.PokerStars:
                case SiteName.PokerStarsFr:
                case SiteName.PokerStarsIt:
                case SiteName.PokerStarsEs:
                    return GetFullHandHistoryParser(siteName);
                case SiteName.Merge:
                    return new MergeFastParserImpl();
                case SiteName.IPoker:
                    return new IPokerFastParserImpl();
                case SiteName.IPoker2:
                    return new IPokerFastParserImpl(true);
                case SiteName.Pacific:
                    return new Poker888FastParserImpl();
                case SiteName.Entraction:
                    return new EntractionFastParserImpl();                    
                case SiteName.OnGame:
                    return new OnGameFastParserImpl();
                case SiteName.OnGameFr:
                    return new OnGameFastParserImpl(SiteName.OnGameFr);
                case SiteName.OnGameIt:
                    return new OnGameFastParserImpl(SiteName.OnGameIt);
                case SiteName.FullTilt:
                    return new FullTiltPokerFastParserImpl();
                case SiteName.MicroGaming:
                    return new MicroGamingFastParserImpl();
                case SiteName.Winamax:
                    return new WinamaxFastParserImpl();
                case SiteName.WinningPoker:
                    return new WinningPokerNetworkFastParserImpl();
                default:
                    throw new NotImplementedException("GetHandHistorySummaryParser: No summary regex parser for " + siteName);
            }
        }
    }
}
