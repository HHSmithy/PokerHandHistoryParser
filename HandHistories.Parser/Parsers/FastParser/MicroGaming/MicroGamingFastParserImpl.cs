﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using System.Globalization;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.AllInAction;
using HandHistories.Parser.Utils.Extensions;
using System.Xml;
using System.IO;
using System.Net;

namespace HandHistories.Parser.Parsers.FastParser.MicroGaming
{
    public sealed partial class MicroGamingFastParserImpl : HandHistoryParserFastImpl
    {
        static readonly CultureInfo provider = CultureInfo.InvariantCulture;

        private static readonly Regex HandSplitRegex = new Regex("(<Game hhversion)", RegexOptions.Compiled);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                                 .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Length > 30)
                                 .Select(s => "<Game hhversion" + s.Trim('\r', 'n'));
        }

        public override SiteName SiteName
        {
            get { return SiteName.MicroGaming; }
        }

        public override bool RequiresAdjustedRaiseSizes
        {
            get { return false; }
        }

        public override bool RequiresAllInDetection
        {
            get { return false; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
        }

        protected override void FinalizeHandHistory(HandHistory Hand)
        {
            FixSitoutPlayers(Hand);
        }

        protected override string[] SplitHandsLines(string handText)
        {
            return Utils.XMLHandLineSplitter.Split(handText);
        }

        static bool GetSittingOutFromPlayerLine(string playerLine)
        {
            return playerLine.IndexOf("sittingout", StringComparison.OrdinalIgnoreCase) != -1;
        }

        private int GetSeatNumberFromPlayerLine(string playerLine)
        {
            string seatNumberString = GetAttribute(playerLine, " num=\"");

            return FastInt.Parse(seatNumberString);
        }

        private bool IsPlayerLineDealer(string playerLine)
        {
            return playerLine.Contains("dealer");
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);

            for (int i = 0; i < playerLines.Count; i++)
            {
                string playerLine = playerLines[i];
                if (IsPlayerLineDealer(playerLine))
                {
                    return GetSeatNumberFromPlayerLine(playerLine);
                }
            }

            /*
             * If its a cancelled hand there is only one player in the hand but he isn't necissarily the dealer.
             * We return -1 to indicate that there is no dealer
             */
            if (playerLines.Count == 1)
            {
                return -1;
            }
            throw new Exception("Could not locate dealer");
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            string dateLine = handLines[0];

            string dateString = GetAttribute(dateLine, " date=\"");

            DateTime dateTime = DateTime.Parse(dateString);

            return dateTime;

        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            var pokerFormat = GetAttribute(handLines[0], " tabletype=\"");

            switch (pokerFormat)
            {
                case "Cash Game":
                    return PokerFormat.CashGame;
                case "MTT":
                    return PokerFormat.MultiTableTournament;
                default:
                    throw new ArgumentException("Unknown pokerFormat: " + pokerFormat);
            }
        }

        protected override long ParseHandId(string[] handLines)
        {
            string handIdLine = handLines[0];

            long handId = long.Parse(GetAttribute(handIdLine, " id=\""));

            return handId;
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        protected override string ParseTableName(string[] handLines)
        {
            string tableNameLine = handLines[0];

            string tableName = GetEncodedAttribute(tableNameLine, " tablename=\"");

            return tableName;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);
            int numPlayers = playerLines.Count();

            if (numPlayers <= 2)
            {
                return SeatType.FromMaxPlayers(2);
            }
            if (numPlayers <= 6)
            {
                return SeatType.FromMaxPlayers(6);
            }
            if (numPlayers <= 9)
            {
                return SeatType.FromMaxPlayers(9);
            }

            return SeatType.FromMaxPlayers(10);
        }

        static List<string> GetPlayerLinesFromHandLines(string[] handLines)
        {
            var playerLines = new List<string>();

            foreach (var line in handLines)
            {
                char TagChar = line[1];
                if (TagChar == 'S' && line[5] == ' ')
                {
                    playerLines.Add(line);
                    continue;
                }
                if (TagChar == '/') break;
            }

            return playerLines;
        }

        private static HoleCards GetPlayerCardsFromHandLines(string[] handLines, int playerSeat, string playerName)
        {
            string seatString = "seat=\"" + playerSeat + "\"";

            for(int i = 8; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line[1] != 'C')
                {
                    continue;
                }

                string showLine = handLines[i - 1];
                if (showLine[1] != 'A')
                {
                    continue;
                }

                int showCardsIndex = showLine.IndexOfFast("type=\"ShowCards\"", 15);
                if (showCardsIndex == -1)
                {
                    showCardsIndex = showLine.IndexOfFast("type=\"MuckCards\"", 15);
                }

                // if the cards are shown for this player
                if (showCardsIndex != -1 && showLine.LastIndexOfFast(seatString) != -1)
                {
                    var cards = ParseCardsFromLines(handLines, ref i);

                    return HoleCards.FromCards(playerName, cards.ToArray());
                }
            }
            return null;
        }

        private static List<Card> ParseCardsFromLines(string[] handLines, ref int i)
        {
            // add all cards the player has ( 2 for hold'em / 4 for omaha )
            List<Card> cards = new List<Card>();
            while(true)
            {
                string line = handLines[i++];
                if (line[1] == '/')
                {
                    break;
                }

                line = line.Replace("value=\"10\"", "value=\"T\"");

                cards.Add(new Card(line[13], line[22]));
            }
            return cards;
        }

        private static List<string> GetCardLinesFromHandLines(string[] handLines)
        {
            var cardLines = new List<string>();
            int startIndex = GetFirstActionIndex(handLines);

            for (int i = startIndex; i < handLines.Length;i++)
            {
                // there will never be more than 5 boardcards
                string actionLine = handLines[i].Trim();

                var actionType = GetActionTypeFromActionLine(actionLine);

                // we need to skip the lines where the players show or muck their cards
                if (actionType.Equals(HandActionType.SHOW) || actionType.Equals(HandActionType.MUCKS) || actionType.Equals(HandActionType.DEALT_HERO_CARDS))
                {
                    do
                    {
                        i++;
                    } while (!handLines[i].StartsWithFast("</Action>"));
                }

                if (actionLine[1] == 'C')
                {
                    cardLines.Add(actionLine);

                    if (cardLines.Count == 5)
                        break;
                }
            }

            return cardLines;
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            string gameTypeLine = handLines[0];

            string gameType = GetEncodedAttribute(gameTypeLine, " gametype=\"");
            string betType = GetEncodedAttribute(gameTypeLine, " betlimit=\"");

            gameType = gameType.Replace("Multi Table ", "");

            return GameTypeUtils.ParseGameString(betType + " " + gameType);
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {

            string gameTypeLine = handLines[0];

            string currencyString = GetEncodedAttribute(gameTypeLine, " currencysymbol=\"");

            Currency currency;

            switch (currencyString)
            {
                case "rCA=":
                    currency = Currency.EURO;
                    break;
                case "":
                    currency = Currency.PlayMoney;
                    break;
                default:
                    throw new LimitException(handLines[0], "Unrecognized currency symbol " + currencyString);
            }

            string limitString = GetAttribute(gameTypeLine, " stakes=\"");

            int slashIndex = limitString.IndexOf('|');

            string smallString = limitString.Substring(0, slashIndex);
            decimal small = decimal.Parse(smallString, System.Globalization.CultureInfo.InvariantCulture);


            string bigString = limitString.Substring(slashIndex + 1, limitString.Length - (slashIndex + 1));
            decimal big = decimal.Parse(bigString, System.Globalization.CultureInfo.InvariantCulture);

            return Limit.FromSmallBlindBigBlind(small, big, currency);
        }

        public override bool IsValidHand(string[] handLines)
        {
            string line0 = handLines[0];
            //Check 1 - hand history version is 4
            if (!line0.Contains("hhversion=\"4\"") && !line0.Contains("hhversion=\"5\""))
            {
                return false;
            }

            //Check 2 - Do we have a Game Tag
            if (line0.StartsWithFast("<Game") == false)
            {
                return false;
            }

            return true;
        }

        static bool isCancelledHand(string[] handLines)
        {
            //Check 3 - Do we have between 2 and 10 players?
            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);
            if (playerLines.Count() < 2 || playerLines.Count() > 10)
            {
                return true;
            }
            return false;
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            if (!IsValidHand(handLines))
            {
                return false;
            }
            else
            {
                isCancelled = isCancelledHand(handLines);
            }
            return true;
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType, out List<WinningsAction> winners)
        {
            var actions = new List<HandAction>();
            winners = new List<WinningsAction>();

            PlayerList playerList = ParsePlayers(handLines, false);

            int firstActionIndex = GetFirstActionIndex(handLines);

            var currentStreet = Street.Preflop;

            for (int i = firstActionIndex; i < handLines.Length - 2; i++)
            {
                string line = handLines[i];

                // skip all non action lines
                if(line[1] != 'A')
                {
                    continue;
                }

                string typeString = GetActionTypeString(line);

                // if the Street changes
                if (typeString.StartsWithFast("deal"))
                {
                    currentStreet = GetStreetFromActionTypeString(typeString);
                    continue;
                }

                switch (typeString)
                {
                    case "muckcards":
                        int seatMuck = GetPlayerSeatFromActionLine(line);
                        string playerNameMuck = playerList.First(p => p.SeatNumber == seatMuck).PlayerName;
                        actions.Add(new HandAction(playerNameMuck, HandActionType.MUCKS, Street.Showdown));
                        continue;

                    case "showcards":
                        int seatShow = GetPlayerSeatFromActionLine(line);
                        string playerNameShow = playerList.First(p => p.SeatNumber == seatShow).PlayerName;
                        actions.Add(new HandAction(playerNameShow, HandActionType.SHOW, Street.Showdown));
                        continue;

                    case "win":
                        winners.AddRange(ParseWinActions(handLines, ref i, playerList));
                        continue;

                    default:
                        break;
                }

                HandAction action = ParseActionFromActionLine(line, currentStreet, playerList, actions); 
                
                if(action != null && !action.HandActionType.Equals(HandActionType.UNKNOWN) && !action.HandActionType.Equals(HandActionType.SHOW))
                {
                    actions.Add(action);
                }
            }

            return actions;

        }

        private IEnumerable<WinningsAction> ParseWinActions(string[] handLines, ref int index, PlayerList players)
        {
            var winners = new List<WinningsAction>();
            for (int i = index + 1; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line.StartsWithFast("</"))
                {
                    index = i + 1;
                    return winners;
                }

                var amount = GetAmountFromActionLine(line);
                int seat = GetSeatNumberFromPlayerLine(line);
                string playerName = players.First(p => p.SeatNumber == seat).PlayerName;

                winners.Add(new WinningsAction(playerName, WinningsActionType.WINS, amount, 0));
            }

            throw new ArgumentOutOfRangeException("WinActions");
        }

        static decimal GetAmountFromActionLine(string line)
        {
            return decimal.Parse(GetAttribute(line, " amount=\""), provider);
        }

        static string GetEncodedAttribute(string line, string name)
        {
            int startIndex = line.IndexOfFast(name) + name.Length;
            int endIndex = line.IndexOf('\"', startIndex);

            return WebUtility.HtmlDecode(line.Substring(startIndex, endIndex - startIndex));
        }

        static string GetAttribute(string line, string name)
        {
            int startIndex = line.IndexOfFast(name) + name.Length;
            int endIndex = line.IndexOf('\"', startIndex + 1);

            return line.Substring(startIndex, endIndex - startIndex);
        }

        static Street GetStreetFromActionTypeString(string typeString)
        {
            switch (typeString)
            {
                case "dealcards":
                    return Street.Preflop;
                case "dealflop":
                    return Street.Flop;
                case "dealturn":
                    return Street.Turn;
                case "dealriver":
                    return Street.River;
                default:
                    throw new ArgumentException("Unknown street: " + typeString);
            }
        }

        public static HandAction ParseActionFromActionLine(string handLine, Street street, PlayerList playerList, List<HandAction> actions)
        {
            bool AllIn = false;
            var actionType = GetActionTypeFromActionLine(handLine);
            if (actionType == HandActionType.UNKNOWN)
            {
                return null;
            }

            int actionNumber = GetActionNumberFromActionLine(handLine);

            if (actionNumber == -1)
                return null;

            decimal value = GetValueFromActionLine(handLine);

            int playerSeat = GetPlayerSeatFromActionLine(handLine);
            string playerName = playerList.First(p => p.SeatNumber == playerSeat).PlayerName;

            if (actionType == HandActionType.ALL_IN)
            {
                AllIn = true;
                bool BBposted = actions.FirstOrDefault(p => p.HandActionType == HandActionType.BIG_BLIND) != null;
                if (!BBposted)
                {
                    actionType = HandActionType.BIG_BLIND;
                }
                bool SBposted = actions.FirstOrDefault(p => p.HandActionType == HandActionType.SMALL_BLIND) != null;
                if (!SBposted)
                {
                    actionType = HandActionType.SMALL_BLIND;
                }

                if (SBposted && BBposted)
                {
                    actionType = AllInActionHelper.GetAllInActionType(playerName, value, street, actions);
                }
            }

            //Bet may occur during preflop, we need to change that to a raise
            if (street == Street.Preflop && actionType == HandActionType.BET)
            {
                actionType = HandActionType.RAISE;
            }

            return new HandAction(playerName, actionType, value, street, AllIn, actionNumber);
        }

        static int GetActionNumberFromActionLine(string actionLine)
        {
            int sequenceStartIndex = actionLine.IndexOfFast(" seq=", 7);
            if (sequenceStartIndex == -1)
            {
                return -1;
            }

            return FastInt.Parse(actionLine, sequenceStartIndex);
        }

        static int GetPlayerSeatFromActionLine(string actionLine)
        {
            int seatStartIndex = actionLine.IndexOfFast(" seat=\"") + 7;

            return FastInt.Parse(actionLine, seatStartIndex);
        }

        static decimal GetValueFromActionLine(string actionLine)
        {
            if (!actionLine.Contains("value"))
                return 0.0m;

            string value = GetAttribute(actionLine, " value=\"");
            
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        static string GetActionTypeString(string line)
        {
            return GetAttribute(line, "type=\"").ToLower();
        }

        static HandActionType GetActionTypeFromActionLine(string actionLine)
        {
            if(char.ToLower(actionLine[1]) != 'a') return HandActionType.UNKNOWN;
            
            var actionType = HandActionType.UNKNOWN;

            switch (GetActionTypeString(actionLine))
            {
                case "smallblind":                
                    actionType = HandActionType.SMALL_BLIND;
                    break;
                case "bigblind":                 
                    actionType = HandActionType.BIG_BLIND;
                    break;
                case "call":                 
                    actionType = HandActionType.CALL;
                    break;
                case "fold":                 
                    actionType = HandActionType.FOLD;
                    break;
                case "check":                 
                    actionType = HandActionType.CHECK;
                    break;
                case "bet":
                    actionType = HandActionType.BET;
                    break;
                case "raise":                 
                    actionType = HandActionType.RAISE;
                    break;
                case "allin":
                    actionType = HandActionType.ALL_IN;
                    break;
                case "showcards":
                    actionType = HandActionType.SHOW;
                    break;
                case "muckcards":
                    actionType = HandActionType.MUCKS;
                    break;
                case "moneyreturned":
                    actionType = HandActionType.UNCALLED_BET;
                    break;
                case "postedtoplay":
                    actionType = HandActionType.POSTS;
                    break;
                case "badbeatcontribution":
                    actionType = HandActionType.JACKPOTCONTRIBUTION;
                    break;
                case "dealcards":
                    actionType = HandActionType.DEALT_HERO_CARDS;
                    break;
                // at the moment we do not need to parse the following types
                case "disconnect":
                case "reconnect":
                case "dealflop":
                case "dealturn":
                case "dealriver":
                    break;

                default:
                    actionType = HandActionType.UNKNOWN;
                    break;
            }

            return actionType;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            return ParsePlayers(handLines, true);
        }

        static PlayerList ParsePlayers(string[] handLines, bool GetHoleCards)
        {
            var playerList = new PlayerList();

            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);

            for (int i = 0; i < playerLines.Count; i++)
            {
                string line = playerLines[i];

                string playerName = GetEncodedAttribute(line, " alias=\"");
                decimal stack = decimal.Parse(GetAttribute(line, " balance=\""), provider);
                int seat = GetPlayerSeatNumber(line);
                bool sittingOut = GetSittingOutFromPlayerLine(playerLines[i]);

                playerList.Add(new Player(playerName, stack, seat)
                {
                    IsSittingOut = sittingOut
                });
            }

            if (!GetHoleCards)
            {
                return playerList;
            }

            int heroSeat = GetHeroSeatNumber(handLines);
            int heroCardsIndex = GetHeroCardsFirstLineIndex(handLines, heroSeat, playerLines.Count + 1);

            if (heroCardsIndex != -1)
            {
                heroCardsIndex++;
                var cards = ParseCardsFromLines(handLines, ref heroCardsIndex);

                var player = playerList.FirstOrDefault(p => p.SeatNumber == heroSeat);
                player.HoleCards = HoleCards.FromCards(player.PlayerName, cards.ToArray());
            }

            foreach (Player player in playerList)
            {
                // try to obtain the holecards for the player
                var holeCards = GetPlayerCardsFromHandLines(handLines, player.SeatNumber, player.PlayerName);
                if (holeCards != null)
                {
                    player.HoleCards = holeCards;
                }
            }

            return playerList;
        }

        static int GetPlayerSeatNumber(string line)
        {
            int startIndex = line.IndexOfFast(" num=\"") + 6;
            return FastInt.Parse(line, startIndex);
        }

        static int GetHeroCardsFirstLineIndex(string[] handLines, int heroSeat, int startIndex)
        {
            string heroDealtToEnd = string.Concat("\"", heroSeat ,"\">");

            for (int i = 0; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line.EndsWithFast(heroDealtToEnd) && line.Contains("type=\"DealCards\""))
                {
                    return i;
                }
                else if (line.EndsWithFast("</Action>"))
                {
                    break;
                }
            }

            return -1;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {            
            List<string> cardLines = GetCardLinesFromHandLines(handLines);

            List<Card> boardCards = new List<Card>(5);

            for (int i = 0; i < cardLines.Count; i++)
            {
                string handLine = cardLines[i];
                handLine = handLine.TrimStart();

                //To make sure we know the exact character location of each card, turn 10s into Ts (these are recognized by our parser)
                handLine = handLine.Replace("value=\"10\"", "value=\"T\"");

                boardCards.Add(new Card(handLine[13], handLine[22]));
            }

            return BoardCards.FromCards(boardCards.ToArray());
        }

        protected override string ParseHeroName(string[] handlines)
        {
            int HeroSeatNumber = GetHeroSeatNumber(handlines);
            
            var player = ParsePlayers(handlines, false).FirstOrDefault(p => p.SeatNumber == HeroSeatNumber);

            if (player != null)
            {
                return player.PlayerName;
            }
            return null;
        }

        private static int GetHeroSeatNumber(string[] handlines)
        {
            string line = handlines[0];
            const string playerSeat = " playerseat=\"";
            string SeatString = GetAttribute(line, playerSeat);
            int HeroSeatNumber = int.Parse(SeatString);
            return HeroSeatNumber;
        }

        private int GetWinningsLineNumberFromHandLines(string[] handLines)
        {
            // for total pot we need to sum up all winnings + rake
            for (int i = handLines.Length - 1; i > 0; i--)
            {
                // search for the win tag
                if (handLines[i].EndsWithFast("Win\">"))
                {
                    return i + 1;
                }
            }
            return -1;
        }

        //this is old: it doesnt produce the right result when it's an uncalled bet
        //protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistory)
        //{
        //    // rake is at the end of the first line of a hand history
        //    string line = handLines[0];
        //    var rakeStartIndex = line.LastIndexOf('=') + 2;
        //    var rakeEndIndex = line.LastIndexOf('"');
        //    var rakeString = line.Substring(rakeStartIndex, rakeEndIndex - rakeStartIndex);

        //    handHistory.Rake = decimal.Parse(rakeString)/100.0m;
        //    handHistory.TotalPot = handHistory.Rake;

        //    int winningsLine = GetWinningsLineNumberFromHandLines(handLines);

        //    // sum up all winnings, amount starts at index 22
        //    for (int k = winningsLine; k < handLines.Length - 1; k++)
        //    {
        //        // leave the loop on </Action>
        //        if (handLines[k][1] == '/')
        //        {
        //            break;
        //        }
        //        string amountString = handLines[k].Substring(22, handLines[k].IndexOf('"', 22) - 22);
        //        handHistory.TotalPot += decimal.Parse(amountString, provider);
        //    }
        //}

        static int GetFirstActionIndex(string[] handLines)
        {
            for (int i = 0; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[1] == 'A')
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
