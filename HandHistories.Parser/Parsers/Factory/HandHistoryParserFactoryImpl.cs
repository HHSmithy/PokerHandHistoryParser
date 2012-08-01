using System;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Compression;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser.Entraction;
using HandHistories.Parser.Parsers.FastParser.IPoker;
using HandHistories.Parser.Parsers.FastParser.Merge;
using HandHistories.Parser.Parsers.FastParser.OnGame;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Parser.Parsers.FastParser._888;
using HandHistories.Parser.Parsers.RegexParser.PartyPoker;

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
                    return new PokerStarsFastParserImpl();
                case SiteName.PokerStarsFr:
                    return new PokerStarsFastParserImpl(SiteName.PokerStarsFr);
                case SiteName.PokerStarsIt:
                    return new PokerStarsFastParserImpl(SiteName.PokerStarsIt);
                case SiteName.Merge:
                    return new MergeFastParserImpl();
                case SiteName.IPoker:
                    return new IPokerFastParserImpl();
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
                    return GetFullHandHistoryParser(siteName);
                case SiteName.Merge:
                    return new MergeFastParserImpl();
                case SiteName.IPoker:
                    return new IPokerFastParserImpl();
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
                default:
                    throw new NotImplementedException("GetHandHistorySummaryParser: No summary regex parser for " + siteName);
            }
        }
    }
}
