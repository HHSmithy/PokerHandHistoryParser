using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using System.Globalization;
using HandHistories.Parser.Utils.FastParsing;

namespace HandHistories.Parser.Parsers.FastParser.MicroGaming
{
    internal sealed class MicroGamingFastParserImpl : HandHistoryParserFastImpl
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

        // TODO: CHECK
        public override bool RequresAdjustedRaiseSizes
        {
            get { return false; }
        }

        // TODO: CHECK
        public override bool RequiresActionSorting
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

        protected override string[] SplitHandsLines(string handText)
        {
            XDocument handDocument = XDocument.Parse(handText);
            return base.SplitHandsLines(handDocument.ToString());
        }

        XDocument GetXDocumentFromLines(string[] handLines)
        {

            string handString = string.Join("", handLines);

            var xrs = new XmlReaderSettings();
            xrs.ConformanceLevel = ConformanceLevel.Fragment;

            var doc = new XDocument(new XElement("root"));
            XElement root = doc.Descendants().First();

            using (var fs = new StringReader(handString))
            using (XmlReader xr = XmlReader.Create(fs, xrs))
            {
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        root.Add(XElement.Load(xr.ReadSubtree()));
                    }
                }
            }

            return doc;
        }

        bool GetSittingOutFromPlayerLine(string playerLine)
        {
            return playerLine.ToLower().Contains("sittingout");
        }

        int GetSeatNumberFromLine(string playerLine)
        {
            string seatNumberString = GetAttribute(playerLine, " num=\"");

            return FastInt.Parse(seatNumberString);
        }

        bool IsPlayerLineDealer(string playerLine)
        {
            return playerLine.Contains("dealer");
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            string[] playerLines = GetPlayerLinesFromHandLines(handLines);

            for (int i = 0; i < playerLines.Count(); i++)
            {
                string playerLine = playerLines[i];
                if (IsPlayerLineDealer(playerLine))
                {
                    return GetSeatNumberFromLine(playerLine);
                }
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

        protected override long ParseHandId(string[] handLines)
        {
            string handIdLine = handLines[0];

            long handId = long.Parse(GetAttribute(handIdLine, " id=\""));

            return handId;
        }

        protected override string ParseTableName(string[] handLines)
        {
            string tableNameLine = handLines[0];

            string tableName = GetAttribute(tableNameLine, " tablename=\"");

            return tableName;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            string[] playerLines = GetPlayerLinesFromHandLines(handLines);
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

        string[] GetPlayerLinesFromHandLines(string[] handLines)
        {
            var playerLines = new List<string>();

            foreach (var line in handLines)
            {
                string playerLine = line.Trim();
                if (playerLine.StartsWith("<Seat num"))
                {
                    playerLines.Add(playerLine);
                    continue;
                }
                if (playerLine.StartsWith("</Seats>")) break;
            }

            return playerLines.ToArray();
        }

        static HoleCards GetPlayerCardsFromHandLines(string[] handLines, int playerSeat, string playerName)
        {

            for(int i = 0; i< handLines.Count(); i++)
            {
                // if the cards are shown for this player
                if(handLines[i].ToLower().Contains("showcards") && handLines[i].Contains(@"seat="""+playerSeat+@""""))
                {
                    // add all cards the player has ( 2 for hold'em / 4 for omaha )
                    var cards = "";
                    do
                    {
                        i++;
                        handLines[i] = handLines[i].Replace("value=\"10\"", "value=\"T\"");

                        cards += handLines[i][13].ToString() + handLines[i][22].ToString();   
                    } while (!handLines[i+1].Contains("</Action"));

                    return HoleCards.FromCards(playerName, cards);
                }
            }
            return null;
        }

        static string[] GetCardLinesFromHandLines(string[] handLines)
        {
            var cardLines = new List<string>();
            for (int i = 0; i < handLines.Count();i++)
            {
                // there will never be more than 5 boardcards
                if (cardLines.Count() == 5) break;

                string actionLine = handLines[i].Trim();

                var actionType = GetActionTypeFromActionLine(actionLine);

                // we need to skip the lines where the players show or muck their cards
                if (actionType.Equals(HandActionType.SHOW) || actionType.Equals(HandActionType.MUCKS))
                {
                    do
                    {
                        i++;
                    } while (!handLines[i].Contains("</Action>"));
                }

                if (actionLine.Contains("<Card "))
                {
                    cardLines.Add(actionLine);
                }
            }

            return cardLines.ToArray();
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            string gameTypeLine = handLines[0];

            string gameType = GetAttribute(gameTypeLine, " gametype=\"");

            string betType = GetAttribute(gameTypeLine, " betlimit=\"");

            return GameTypeUtils.ParseGameString(betType + " " + gameType);
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {

            string gameTypeLine = handLines[0];

            string currencyString = GetAttribute(gameTypeLine, " currencysymbol=\"");

            Currency currency;

            // TODO: check and add more currencies
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

            //Check 1 - hand history version is 4
            if (handLines[0].Contains("hhversion=\"4\"") == false)
            {
                return false;
            }

            //Check 2 - Do we have a Game Tag
            if (handLines[0].StartsWith("<Game") == false)
            {
                return false;
            }

            //Check 3 - Do we have between 2 and 10 players?
            string[] playerLines = GetPlayerLinesFromHandLines(handLines);
            if (playerLines.Count() < 2 || playerLines.Count() > 10)
            {
                return false;
            }

            return true;
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            var actions = new List<HandAction>();

            PlayerList playerList = ParsePlayers(handLines);
            
            var currentStreet = Street.Preflop;

            for (int i = 0; i < handLines.Length - 2; i++)
            {
                string line = handLines[i].TrimStart();

                // skip all non action lines
                if(!line.Contains("<Action"))
                {
                    continue;
                }

                string typeString = GetActionTypeString(line);

                // if the Street changes
                if (typeString.StartsWith("deal", StringComparison.Ordinal))
                {
                    currentStreet = GetStreetFromActionTypeString(typeString);
                    continue;
                }

                switch (typeString)
                {
                    case "showcards":
                        int seat = GetPlayerSeatFromActionLine(line);
                        string playerName = playerList.First(p => p.SeatNumber == seat).PlayerName;
                        actions.Add(new HandAction(playerName, HandActionType.SHOW, Street.Showdown));
                        continue;

                    case "win":
                        actions.AddRange(ParseWinActions(handLines, ref i, playerList));
                        continue;

                    default:
                        break;
                }

                HandAction action = GetHandActionFromActionLine(line, currentStreet, playerList); 
                
                if(action != null && !action.HandActionType.Equals(HandActionType.UNKNOWN) && !action.HandActionType.Equals(HandActionType.SHOW))
                {
                    actions.Add(action);
                }
            }

            return actions;

        }

        private IEnumerable<HandAction> ParseWinActions(string[] handLines, ref int index, PlayerList players)
        {
            List<HandAction> actions = new List<HandAction>();
            for (int i = index + 1; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line.StartsWith("</", StringComparison.Ordinal))
                {
                    index = i + 1;
                    return actions;
                }

                var amount = GetAmountFromActionLine(line);
                int seat = GetSeatNumberFromLine(line);
                string playerName = players.First(p => p.SeatNumber == seat).PlayerName;

                actions.Add(new WinningsAction(playerName, HandActionType.WINS, amount, 0));
            }

            throw new ArgumentOutOfRangeException("WinActions");
        }

        static decimal GetAmountFromActionLine(string line)
        {
            return decimal.Parse(GetAttribute(line, " amount=\""), provider);
        }

        static string GetAttribute(string line, string name)
        {
            int startIndex = line.IndexOf(name) + name.Length;
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
                    throw new ArgumentException("typeString");
            }
        }

        private HandAction GetHandActionFromActionLine(string handLine, Street street, PlayerList playerList)
        {
            var actionType = GetActionTypeFromActionLine(handLine);
            
            decimal value = GetValueFromActionLine(handLine);
            int actionNumber = GetActionNumberFromActionLine(handLine);

            if (actionNumber == -1 || actionType == HandActionType.UNKNOWN)
                return null;

            int playerSeat = GetPlayerSeatFromActionLine(handLine);

            return new HandAction(playerList.First(p => p.SeatNumber == playerSeat).PlayerName, actionType, value, street, actionNumber);
        }

        static int GetActionNumberFromActionLine(string actionLine)
        {
            if (!actionLine.ToLower().Contains(" seq=")) return -1;

            string actionNumString = GetAttribute(actionLine, " seq=\"");
            
            return Int32.Parse(actionNumString);
        }

        static int GetPlayerSeatFromActionLine(string actionLine)
        {
            string seat = GetAttribute(actionLine, " seat=\"");

            return FastInt.Parse(seat);
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
            if(!actionLine.ToLower().Contains("<action ")) return HandActionType.UNKNOWN;
            
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
                case "win":
                    actionType = HandActionType.WINS;
                    break;
                case "postedtoplay":
                case "badbeatcontribution":
                    actionType = HandActionType.POSTS;
                    break;
                // at the moment we do not need to parse the following types
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
            var playerList = new PlayerList();

            string[] playerLines = GetPlayerLinesFromHandLines(handLines);

            for (int i = 0; i < playerLines.Length; i++)
            {
                string line = playerLines[i];

                string playerName = GetAttribute(line, " alias=\"");
                decimal stack = decimal.Parse(GetAttribute(line, " balance=\""), provider);
                int seat = int.Parse(GetAttribute(line, " num=\""));
                bool sittingOut = GetSittingOutFromPlayerLine(playerLines[i]);

                playerList.Add(new Player(playerName, stack, seat)
                                   {
                                       IsSittingOut = sittingOut
                                   });
            }

            foreach (Player player in playerList)
            {
                // try to obtain the holecards for the player
                var holeCards = GetPlayerCardsFromHandLines(handLines, player.SeatNumber, player.PlayerName);
                if(holeCards != null)
                {
                    player.HoleCards = holeCards;
                }
            }

            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {            
            string[] cardLines = GetCardLinesFromHandLines(handLines);

            string boardCards = "";

            for (int i = 0; i < cardLines.Length; i++)
            {
                string handLine = cardLines[i];
                handLine = handLine.TrimStart();

                //To make sure we know the exact character location of each card, turn 10s into Ts (these are recognized by our parser)
                handLine = handLine.Replace("value=\"10\"", "value=\"T\"");

                boardCards += new Card(handLine[13], handLine[22]);
            }

            return BoardCards.FromCards(boardCards);
        }

        protected override string ParseHeroName(string[] handlines)
        {
            string line = handlines[0];
            const string playerSeat = " playerseat=\"";
            string SeatString = GetAttribute(line, playerSeat);
            int HeroSeatNumber = int.Parse(SeatString);
            
            var player = ParsePlayers(handlines).FirstOrDefault(p => p.SeatNumber == HeroSeatNumber);

            if (player != null)
            {
                return player.PlayerName;
            }
            return null;
        }
    }
}
