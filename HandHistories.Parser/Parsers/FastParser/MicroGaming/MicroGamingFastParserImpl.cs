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

namespace HandHistories.Parser.Parsers.FastParser.MicroGaming
{
    internal sealed class MicroGamingFastParserImpl : HandHistoryParserFastImpl
    {
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

        protected override string[] SplitHandsLines(string handText)
        {
            XDocument handDocument = XDocument.Parse(handText);
            return base.SplitHandsLines(handDocument.ToString());
        }

        protected XDocument GetXDocumentFromLines(string[] handLines)
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

        protected bool GetSittingOutFromPlayerLine(string playerLine)
        {
            return playerLine.ToLower().Contains("sittingout");
        }

        protected int GetSeatNumberFromPlayerLine(string playerLine)
        {
            int startPos = playerLine.IndexOf("num=", StringComparison.Ordinal) + 5;
            int endPos = playerLine.IndexOf('"', startPos);

            string seatNumberString = playerLine.Substring(startPos, endPos - startPos);

            return Int32.Parse(seatNumberString);

        }

        protected bool IsPlayerLineDealer(string playerLine)
        {
            return playerLine.Contains("dealer");
        }

        protected decimal GetStackFromPlayerLine(string playerLine)
        {
            int startPos = playerLine.IndexOf(@" balance=", StringComparison.Ordinal) + 10;
            int endPost = playerLine.IndexOf('"', startPos) - 1;

            string stackString = playerLine.Substring(startPos, endPost - startPos + 1);

            return decimal.Parse(stackString, System.Globalization.CultureInfo.InvariantCulture);
        }

        protected decimal GetWinningsFromPlayerLine(string playerLine)
        {
            if (playerLine.ToLower().Contains("sittingout")) return 0.0m;

            int startPos = playerLine.IndexOf(@" endbalance=", StringComparison.Ordinal) + 13;
            int endPos = playerLine.IndexOf('"', startPos) - 1;

            string stackString = playerLine.Substring(startPos, endPos - startPos + 1);

            decimal startStack = GetStackFromPlayerLine(playerLine);

            return (decimal.Parse(stackString, System.Globalization.CultureInfo.InvariantCulture) - startStack);
        }

        protected string GetNameFromPlayerLine(string playerLine)
        {
            int startPos = playerLine.IndexOf(@" alias=", StringComparison.Ordinal) + 8;
            int endPos = @playerLine.IndexOf("\"", startPos, StringComparison.Ordinal) - 1;

            string name = playerLine.Substring(startPos, endPos - startPos + 1);

            return name;
        }


        protected override int ParseDealerPosition(string[] handLines)
        {
            string[] playerLines = GetPlayerLinesFromHandLines(handLines);

            for (int i = 0; i < playerLines.Count(); i++)
            {
                string playerLine = playerLines[i];
                if (IsPlayerLineDealer(playerLine))
                {
                    return GetSeatNumberFromPlayerLine(playerLine);
                }
            }
            throw new Exception("Could not locate dealer");
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            string dateLine = handLines[0];

            int startPos = dateLine.IndexOf(@" date=", StringComparison.Ordinal) + 7;
            int endPos = dateLine.IndexOf('"', startPos) - 1;

            string dateString = dateLine.Substring(startPos, endPos - startPos + 1);

            DateTime dateTime = DateTime.Parse(dateString);

            return dateTime;

        }

        protected override long ParseHandId(string[] handLines)
        {
            string handIdLine = handLines[0];

            int startPos = handIdLine.IndexOf(@" id=", StringComparison.Ordinal) + 5;
            int endPos = handIdLine.IndexOf('"', startPos) - 1;

            long handId = long.Parse(handLines[0].Substring(startPos, endPos - startPos + 1));

            return handId;
        }

        protected override string ParseTableName(string[] handLines)
        {
            string tableNameLine = handLines[0];

            int startPos = tableNameLine.IndexOf(@" tablename=",StringComparison.Ordinal) + 12;
            int endPos = tableNameLine.IndexOf('"', startPos) - 1;

            string tableName = tableNameLine.Substring(startPos, endPos - startPos + 1);

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

        protected string[] GetPlayerLinesFromHandLines(string[] handLines)
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

        protected HoleCards GetPlayerCardsFromHandLines(string[] handLines, int playerSeat, string playerName)
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

        protected string[] GetCardLinesFromHandLines(string[] handLines)
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

            int gameTypeStartPos = gameTypeLine.IndexOf(@" gametype=", StringComparison.Ordinal) + 11;
            int gameTypeEndPos = gameTypeLine.IndexOf('"', gameTypeStartPos) - 1;

            string gameType = gameTypeLine.Substring(gameTypeStartPos, gameTypeEndPos - gameTypeStartPos + 1);

            int betTypeStartPos = gameTypeLine.IndexOf(@" betlimit=", StringComparison.Ordinal) + 11;
            int betTypeEndPos = gameTypeLine.IndexOf('"', betTypeStartPos) - 1;

            string betType = gameTypeLine.Substring(betTypeStartPos, betTypeEndPos - betTypeStartPos + 1);

            return GameTypeUtils.ParseGameString(betType + " " + gameType);
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {

            string gameTypeLine = handLines[0];

            int currencyStartPos = gameTypeLine.IndexOf(" currencysymbol=", StringComparison.Ordinal) + 17;
            int currencyEndPos = gameTypeLine.IndexOf('"', currencyStartPos) - 1;

            string currencyString = gameTypeLine.Substring(currencyStartPos, currencyEndPos - currencyStartPos + 1);

            Currency currency;

            // TODO: check and add more currencies
            switch (currencyString)
            {
                case "rCA=":
                    currency = Currency.EURO;
                    break;
                default:
                    throw new LimitException(handLines[0], "Unrecognized currency symbol " + currencyString);
            }


            int startPos = gameTypeLine.IndexOf(" stakes=", StringComparison.Ordinal) + 9;
            int endPos = gameTypeLine.IndexOf('"', startPos) - 1;

            string limitString = gameTypeLine.Substring(startPos, endPos - startPos + 1);

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
            
            var currentStreet = Street.Preflop;

            for (int i = 0; i < handLines.Length - 2; i++)
            {
                string handLine = handLines[i].TrimStart();

                // skip all non action lines
                if(!handLine.Contains("<Action"))
                {
                    continue;
                }

                // if the Street changes
                if (handLine.Contains("type=\"Deal"))
                {
                    currentStreet = GetStreetFromHandLine(handLine);
                    continue;
                }

                HandAction action = GetHandActionFromActionLine(handLine, currentStreet); 
                
                if(action != null && !action.HandActionType.Equals(HandActionType.UNKNOWN) && !action.HandActionType.Equals(HandActionType.SHOW))
                {
                    actions.Add(action);
                }
            }

            //Generate the show card + winnings actions
            actions.AddRange(GetWinningAndShowCardActions(handLines));
           
            return actions;

        }

        private List<HandAction> GetWinningAndShowCardActions(string[] handLines)
        {

            int actionNumber = Int32.MaxValue - 100;

            PlayerList playerList = ParsePlayers(handLines);

            var winningAndShowCardActions = new List<HandAction>();
            
            foreach (Player player in playerList)
            {
                if (player.hasHoleCards)
                {
                    HandAction showCardsAction = new HandAction(player.PlayerName, HandActionType.SHOW, 0, Street.Showdown, actionNumber++);    
                    winningAndShowCardActions.Add(showCardsAction);
                }                
            }

            string[] playerLines = GetPlayerLinesFromHandLines(handLines);
            for (int i = 0; i < playerLines.Length; i++)
            {
                string playerLine = playerLines[i];
                decimal winnings = GetWinningsFromPlayerLine(playerLine);                
                if (winnings > 0)
                {
                    string playerName = GetNameFromPlayerLine(playerLine);
                    WinningsAction winningsAction = new WinningsAction(playerName, HandActionType.WINS, winnings, 0, actionNumber++);
                    winningAndShowCardActions.Add(winningsAction);
                }
            }

            return winningAndShowCardActions;
        }

        private HandAction GetHandActionFromActionLine(string handLine, Street street)
        {
            
            var actionType = GetActionTypeFromActionLine(handLine);
            string actionPlayerSeat = GetPlayerSeatFromActionLine(handLine);

            decimal value = GetValueFromActionLine(handLine);
            int actionNumber = GetActionNumberFromActionLine(handLine);

            if (actionNumber == -1)
                return null;

            return new HandAction(actionPlayerSeat, actionType, value, street, actionNumber);

        }

        protected Street GetStreetFromHandLine(string handLine)
        {
            if(handLine.Contains("type=\"DealFlop\""))
            {
                return Street.Flop;
            }
            if(handLine.Contains("type=\"DealTurn\""))
            {
                return Street.Turn;
            }
            if(handLine.Contains("type=\"DealRiver\""))
            {
                return Street.River;
            }
            return Street.Null;
        }

        protected int GetActionNumberFromActionLine(string actionLine)
        {
            if (!actionLine.ToLower().Contains(" seq=")) return -1;

            int startPos = actionLine.IndexOf(@" seq=", StringComparison.Ordinal) + 6;
            int endPos = actionLine.IndexOf('"', startPos) - 1;
            
            string actionNumString = actionLine.Substring(startPos, endPos - startPos + 1);
            
            return Int32.Parse(actionNumString);
            
        }

        protected string GetPlayerSeatFromActionLine(string actionLine)
        {
            if(!actionLine.Contains("seat"))
                return null;

            int startPos = actionLine.IndexOf(@" seat=",StringComparison.Ordinal) + 7;
            int endPos = actionLine.IndexOf('"', startPos) - 1;
            
            string seat = actionLine.Substring(startPos, endPos - startPos + 1);

            return seat;
        }

        protected decimal GetValueFromActionLine(string actionLine)
        {
            if (!actionLine.Contains("value"))
                return 0.0m;

            int startPos = actionLine.IndexOf(@" value=",StringComparison.Ordinal) + 8;
            int endPos = actionLine.IndexOf('"', startPos) - 1;
            
            string value = actionLine.Substring(startPos, endPos - startPos + 1);
            
            return decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        protected HandActionType GetActionTypeFromActionLine(string actionLine)
        {
            if(!actionLine.ToLower().Contains("<action ")) return HandActionType.UNKNOWN;
            
            int startPos = actionLine.IndexOf(@" type=",StringComparison.Ordinal) + 7;
            int endPos = actionLine.IndexOf('"', startPos) - 1;
            
            string actionString = actionLine.Substring(startPos, endPos - startPos + 1);

            var actionType = HandActionType.UNKNOWN;

            switch (actionString.ToLower())
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
                string playerName = GetNameFromPlayerLine(playerLines[i]);
                decimal stack = GetStackFromPlayerLine(playerLines[i]);
                int seat = GetSeatNumberFromPlayerLine(playerLines[i]);
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
            const string playerSeat = "playerseat=\"";
            int PlayerSeatStartIndex = line.IndexOf(playerSeat) + playerSeat.Length;
            int PlayerSeatEndIndex = line.IndexOf("\"", PlayerSeatStartIndex);
            string SeatString = line.Substring(PlayerSeatStartIndex, PlayerSeatEndIndex - PlayerSeatStartIndex);
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
