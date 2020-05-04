using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Utils.AllInAction;
using HandHistories.Parser.Utils.Pot;
using HandHistories.Parser.Utils.RaiseAdjuster;
using HandHistories.Parser.Utils.Uncalled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace HandHistories.Parser.Parsers.JSONParser.Base
{
    public abstract class HandHistoryParserJSONImpl : IHandHistoryParser
    {
        private static readonly Regex HandSplitRegex = new Regex("\r\n\r\n", RegexOptions.Compiled);

        public abstract SiteName SiteName { get; }

        public IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .Select(s => s.Trim('\r', '\n'));
        }

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            var JSON = GetJSONObject(handText);

            HandHistorySummary summary = new HandHistorySummary();

            summary.DateOfHandUtc = ParseDateUtc(JSON);
            summary.DealerButtonPosition = ParseDealerPosition(JSON);
            summary.FullHandHistoryText = handText;
            summary.HandId = ParseHandId(JSON);
            summary.NumPlayersSeated = ParseNumPlayers(JSON);
            summary.TableName = ParseTableName(JSON);
            summary.GameDescription = ParseGameDescriptor(JSON);

            ParseExtraHandInformation(JSON, summary);

            return summary;
        }

        public HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false)
        {
            var JSON = GetJSONObject(handText);

            HandHistory hand = new HandHistory();

            hand.DateOfHandUtc = ParseDateUtc(JSON);
            hand.DealerButtonPosition = ParseDealerPosition(JSON);
            hand.FullHandHistoryText = handText;
            hand.HandId = ParseHandId(JSON);
            hand.NumPlayersSeated = ParseNumPlayers(JSON);
            hand.TableName = ParseTableName(JSON);
            hand.Players = ParsePlayers(JSON);
            hand.HandActions = AdjustHandActions(ParseHandActions(JSON));
            hand.Winners = ParseWinners(JSON);
            hand.GameDescription = ParseGameDescriptor(JSON);
            hand.CommunityCards = ParseCommunityCards(JSON);

            string heroName = ParseHeroName(JSON);
            hand.Hero = hand.Players.FirstOrDefault(p => p.PlayerName == heroName);

            ParseExtraHandInformation(JSON, hand);

            FinalizeHandHistory(hand);

            return hand;
        }

        protected virtual void FinalizeHandHistory(HandHistory hand)
        {
        }

        protected virtual void ParseExtraHandInformation(JObject JSON, HandHistorySummary handHistorySummary)
        {
            // do nothing
        }

        GameDescriptor ParseGameDescriptor(JObject JSON)
        {
            var format = ParsePokerFormat(JSON);
            var gametype = ParseGameType(JSON);
            var limit = ParseLimit(JSON);
            var tabletype = ParseTableType(JSON);
            var seattype = ParseSeatType(JSON);

            return new GameDescriptor(format, SiteName.IGT, gametype, limit, tabletype, seattype);
        }

        public GameDescriptor ParseGameDescriptor(string handText)
        {
            var JSON = GetJSONObject(handText);

            return ParseGameDescriptor(JSON);
        }

        protected abstract PokerFormat ParsePokerFormat(JObject JSON);

        public PokerFormat ParsePokerFormat(string handText)
        {
            return ParsePokerFormat(GetJSONObject(handText));
        }

        protected abstract List<HandAction> ParseHandActions(JObject JSON);

        protected abstract List<WinningsAction> ParseWinners(JObject JSON);

        public List<HandAction> ParseHandActions(string handText, out List<WinningsAction> winners)
        {
            var JSON = GetJSONObject(handText);
            winners = ParseWinners(JSON);
            return AdjustHandActions(ParseHandActions(JSON));
        }

        protected virtual List<HandAction> AdjustHandActions(List<HandAction> actions)
        {
            return actions;
        }

        protected abstract PlayerList ParsePlayers(JObject JSON);

        public PlayerList ParsePlayers(string handText)
        {
            return ParsePlayers(GetJSONObject(handText));
        }

        protected abstract BoardCards ParseCommunityCards(JObject JSON);

        public BoardCards ParseCommunityCards(string handText)
        {
            return ParseCommunityCards(GetJSONObject(handText));
        }

        protected abstract int ParseDealerPosition(JObject JSON);

        public int ParseDealerPosition(string handText)
        {
            return ParseDealerPosition(GetJSONObject(handText));
        }

        protected abstract DateTime ParseDateUtc(JObject JSON);

        public DateTime ParseDateUtc(string handText)
        {
            return ParseDateUtc(GetJSONObject(handText));
        }

        protected abstract long[] ParseHandId(JObject JSON);

        public long[] ParseHandId(string handText)
        {
            return ParseHandId(GetJSONObject(handText));
        }

        protected abstract string ParseTableName(JObject JSON);

        public string ParseTableName(string handText)
        {
            return ParseTableName(GetJSONObject(handText));
        }

        protected abstract SeatType ParseSeatType(JObject JSON);

        public SeatType ParseSeatType(string handText)
        {
            return ParseSeatType(GetJSONObject(handText));
        }

        protected abstract GameType ParseGameType(JObject JSON);

        public GameType ParseGameType(string handText)
        {
            return ParseGameType(GetJSONObject(handText));
        }

        protected abstract TableType ParseTableType(JObject JSON);

        public TableType ParseTableType(string handText)
        {
            return ParseTableType(GetJSONObject(handText));
        }

        protected abstract Limit ParseLimit(JObject JSON);

        public Limit ParseLimit(string handText)
        {
            return ParseLimit(GetJSONObject(handText));
        }

        protected abstract int ParseNumPlayers(JObject JSON);

        public int ParseNumPlayers(string handText)
        {
            return ParseNumPlayers(GetJSONObject(handText));
        }

        protected abstract string ParseHeroName(JObject JSON);

        public string ParseHeroName(string handText)
        {
            return ParseHeroName(GetJSONObject(handText));
        }

        public bool IsValidHand(string handText)
        {
            throw new NotImplementedException();
        }

        public bool IsValidOrCancelledHand(string handText, out bool isCancelled)
        {
            throw new NotImplementedException();
        }

        static JObject GetJSONObject(string text)
        {
            return JObject.Parse(text);
        }
    }
}
