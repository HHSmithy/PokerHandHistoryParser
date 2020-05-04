using HandHistories.Parser.Parsers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;

namespace HandHistories.Parser.Parsers
{
    public class MultiVersionParser : IHandHistoryParser
    {
        List<Tuple<Func<string, bool>, IHandHistoryParser>> Parsers = new List<Tuple<Func<string, bool>, IHandHistoryParser>>();

        IHandHistoryParser GetParser(string handtext)
        {
            return Parsers.First(p => p.Item1(handtext)).Item2;
        }

        public void Add(IHandHistoryParser parser, Func<string, bool> filter)
        {
            Parsers.Add(new Tuple<Func<string, bool>, IHandHistoryParser>(filter, parser));
        }

        public SiteName SiteName { get; set; }

        public bool IsValidHand(string handText)
        {
            return GetParser(handText).IsValidHand(handText);
        }

        public bool IsValidOrCancelledHand(string handText, out bool isCancelled)
        {
            return GetParser(handText).IsValidOrCancelledHand(handText, out isCancelled);
        }

        public BoardCards ParseCommunityCards(string handText)
        {
            return GetParser(handText).ParseCommunityCards(handText);
        }

        public DateTime ParseDateUtc(string handText)
        {
            return GetParser(handText).ParseDateUtc(handText);
        }

        public int ParseDealerPosition(string handText)
        {
            return GetParser(handText).ParseDealerPosition(handText);
        }

        public HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false)
        {
            return GetParser(handText).ParseFullHandHistory(handText, rethrowExceptions);
        }

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            return GetParser(handText).ParseFullHandSummary(handText, rethrowExceptions);
        }

        public GameDescriptor ParseGameDescriptor(string handText)
        {
            return GetParser(handText).ParseGameDescriptor(handText);
        }

        public GameType ParseGameType(string handText)
        {
            return GetParser(handText).ParseGameType(handText);
        }

        public List<HandAction> ParseHandActions(string handText, out List<WinningsAction> winners)
        {
            return GetParser(handText).ParseHandActions(handText, out winners);
        }

        public long[] ParseHandId(string handText)
        {
            return GetParser(handText).ParseHandId(handText);
        }

        public string ParseHeroName(string handText)
        {
            return GetParser(handText).ParseHeroName(handText);
        }

        public Limit ParseLimit(string handText)
        {
            return GetParser(handText).ParseLimit(handText);
        }

        public int ParseNumPlayers(string handText)
        {
            return GetParser(handText).ParseNumPlayers(handText);
        }

        public PlayerList ParsePlayers(string handText)
        {
            return GetParser(handText).ParsePlayers(handText);
        }

        public PokerFormat ParsePokerFormat(string handText)
        {
            return GetParser(handText).ParsePokerFormat(handText);
        }

        public SeatType ParseSeatType(string handText)
        {
            return GetParser(handText).ParseSeatType(handText);
        }

        public string ParseTableName(string handText)
        {
            return GetParser(handText).ParseTableName(handText);
        }

        public TableType ParseTableType(string handText)
        {
            return GetParser(handText).ParseTableType(handText);
        }

        public IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return GetParser(rawHandHistories).SplitUpMultipleHands(rawHandHistories);
        }
    }
}
