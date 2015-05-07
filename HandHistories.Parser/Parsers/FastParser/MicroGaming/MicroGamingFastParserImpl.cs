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

        public override bool RequiresAdjustedRaiseSizes
        {
            get { return false; }
        }

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

        private bool GetSittingOutFromPlayerLine(string playerLine)
        {
            return playerLine.ToLower().Contains("sittingout");
        }

        private int GetSeatNumberFromPlayerLine(string playerLine)
        {
            int startPos = playerLine.IndexOf("num=", StringComparison.Ordinal) + 5;
            int endPos = playerLine.IndexOf('"', startPos);

            string seatNumberString = playerLine.Substring(startPos, endPos - startPos);

            return Int32.Parse(seatNumberString);

        }

        private bool IsPlayerLineDealer(string playerLine)
        {
            return playerLine.Contains("dealer");
        }

        private decimal GetStackFromPlayerLine(string playerLine)
        {
            int startPos = playerLine.IndexOf(@" balance=", StringComparison.Ordinal) + 10;
            int endPost = playerLine.IndexOf('"', startPos) - 1;

            string stackString = playerLine.Substring(startPos, endPost - startPos + 1);

            return decimal.Parse(stackString, System.Globalization.CultureInfo.InvariantCulture);
        }

        private decimal GetWinningsFromPlayerLine(string playerLine)
        {
            if (playerLine.ToLower().Contains("sittingout")) return 0.0m;

            int startPos = playerLine.IndexOf(@" endbalance=", StringComparison.Ordinal) + 13;
            int endPos = playerLine.IndexOf('"', startPos) - 1;

            string stackString = playerLine.Substring(startPos, endPos - startPos + 1);

            decimal startStack = GetStackFromPlayerLine(playerLine);

            return (decimal.Parse(stackString, System.Globalization.CultureInfo.InvariantCulture) - startStack);
        }

        private string GetNameFromPlayerLine(string playerLine)
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

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            return PokerFormat.CashGame;
        }

        protected override long ParseHandId(string[] handLines)
        {
            string handIdLine = handLines[0];

            int startPos = handIdLine.IndexOf(@" id=", StringComparison.Ordinal) + 5;
            int endPos = handIdLine.IndexOf('"', startPos) - 1;

            long handId = long.Parse(handLines[0].Substring(startPos, endPos - startPos + 1));

            return handId;
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
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

        private string[] GetPlayerLinesFromHandLines(string[] handLines)
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

        private HoleCards GetPlayerCardsFromHandLines(string[] handLines, int playerSeat, string playerName)
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

        private string[] GetCardLinesFromHandLines(string[] handLines)
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

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
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

            // we need the player infos for action assignments
            var players = ParsePlayers(handLines);

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

                HandAction action = GetHandActionFromActionLine(handLine, currentStreet, players); 
                
                if(action != null && !action.HandActionType.Equals(HandActionType.UNKNOWN) && !action.HandActionType.Equals(HandActionType.SHOW))
                {
                    actions.Add(action);
                }
            }

            //Generate the show card + winnings actions
            actions.AddRange(GetWinningAndShowCardActions(handLines, actions, players));
           
            return FixPostedToPlayActions(handLines, actions);

        }

        // rarely it happens that postedToPlay actions have a wrong amount included ( e.g. only the BB although the player posted SB+BB )
        private List<HandAction> FixPostedToPlayActions(string[] handLines, List<HandAction> actions)
        {
            var playersWhoPosted = actions.Where(a => a.HandActionType.Equals(HandActionType.POSTS)).Select(h => h.PlayerName).ToList();

            foreach (var player in playersWhoPosted)
            {
                // compare start / end balance with player results and modify the playedToPost accordingly
                var line = handLines.First(a => a.Contains(player));
                var parsedPlayerResult = GetWinningsFromPlayerLine(line);
                var actionResult = actions.Where(a => a.PlayerName.Equals(player)).Sum(a => a.Amount);
                var difference = actionResult - parsedPlayerResult;

                var sb = Math.Abs(actions.First(a => a.HandActionType.Equals(HandActionType.SMALL_BLIND)).Amount);

                // a player might also contribute to the jackpot, which is not included in our handactions, but in the parsedplayerresult
                if (difference == sb || difference == sb + 0.02m)
                {
                    var oldAction = actions.First(a => a.PlayerName.Equals(player) && a.HandActionType.Equals(HandActionType.POSTS));
                    var newPostedToPlayAction = new HandAction(player, HandActionType.POSTS, Math.Abs(oldAction.Amount) + sb,Street.Preflop, oldAction.ActionNumber);
                    actions.Remove(oldAction);
                    actions.Add(newPostedToPlayAction);
                }
            }

            return actions;
        }

        private List<HandAction> GetWinningAndShowCardActions(string[] handLines, List<HandAction> actions, PlayerList playerList)
        {

            int actionNumber = actions.Max(a => a.ActionNumber);
            
            var winningAndShowCardActions = new List<HandAction>();
            
            foreach (Player player in playerList)
            {
                if (player.hasHoleCards)
                {
                    HandAction showCardsAction = new HandAction(player.PlayerName, HandActionType.SHOW, 0, Street.Showdown, actionNumber++);    
                    winningAndShowCardActions.Add(showCardsAction);
                }                
            }

            int winningsLine = GetWinningsLineNumberFromHandLines(handLines);

            for (int k = winningsLine; k < handLines.Length - 1; k++)
            {
                // leave the loop on </Action>
                if (handLines[k][1] == '/')
                {
                    break;
                }

                var actionPlayerSeat = GetPlayerSeatFromActionLine(handLines[k]);
                var amountString = handLines[k].Substring(22, handLines[k].IndexOf('"', 22) - 22);

                winningAndShowCardActions.Add(new HandAction(GetPlayerNameFromSeatNumber(actionPlayerSeat, playerList),HandActionType.WINS, decimal.Parse(amountString), Street.Showdown, actionNumber++));
            }
           

            return winningAndShowCardActions;
        }

        private HandAction GetHandActionFromActionLine(string handLine, Street street, PlayerList playerList)
        {
            
            var actionType = GetActionTypeFromActionLine(handLine);
            var actionPlayerSeat = GetPlayerSeatFromActionLine(handLine);

            decimal value = GetValueFromActionLine(handLine);
            
            int actionNumber = GetActionNumberFromActionLine(handLine);

            if (actionNumber == -1)
                return null;

            return new HandAction(GetPlayerNameFromSeatNumber(actionPlayerSeat, playerList), actionType, value, street, actionNumber);
        }

        private string GetPlayerNameFromSeatNumber(int seat, PlayerList playerList)
        {
            var player = playerList.FirstOrDefault(a => a.SeatNumber == seat);
            
            if (player == null || seat == -1)
                return "UNKNOWN";

            return player.PlayerName;
        }

        private Street GetStreetFromHandLine(string handLine)
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

        private int GetActionNumberFromActionLine(string actionLine)
        {
            if (!actionLine.ToLower().Contains(" seq=")) return -1;

            int startPos = actionLine.IndexOf(@" seq=", StringComparison.Ordinal) + 6;
            int endPos = actionLine.IndexOf('"', startPos) - 1;
            
            string actionNumString = actionLine.Substring(startPos, endPos - startPos + 1);
            
            return Int32.Parse(actionNumString);
            
        }

        private int GetPlayerSeatFromActionLine(string actionLine)
        {
            if(!actionLine.ToLower().Contains("seat"))
                return -1;

            int startPos = actionLine.IndexOf(@" seat=",StringComparison.Ordinal) + 7;

            if (startPos == 6) // which means not found ( -1 + 7 )
                startPos = actionLine.IndexOf(@" num=", StringComparison.Ordinal) + 6;

            int endPos = actionLine.IndexOf('"', startPos) - 1;
            
            string seat = actionLine.Substring(startPos, endPos - startPos + 1);

            return Int32.Parse(seat);
        }

        private decimal GetValueFromActionLine(string actionLine)
        {
            if (!actionLine.Contains("value"))
                return 0.0m;

            int startPos = actionLine.IndexOf(@" value=",StringComparison.Ordinal) + 8;
            int endPos = actionLine.IndexOf('"', startPos) - 1;
            
            string value = actionLine.Substring(startPos, endPos - startPos + 1);
            
            return decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private HandActionType GetActionTypeFromActionLine(string actionLine)
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
                    actionType = HandActionType.POSTS;
                    break;
                case "badbeatcontribution":
                    actionType = HandActionType.JACKPOTCONTRIBUTION;
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
            int playerSeatStartIndex = line.IndexOf(playerSeat, StringComparison.Ordinal) + playerSeat.Length;
            int playerSeatEndIndex = line.IndexOf("\"", playerSeatStartIndex, StringComparison.Ordinal);
            string seatString = line.Substring(playerSeatStartIndex, playerSeatEndIndex - playerSeatStartIndex);
            int heroSeatNumber = int.Parse(seatString);
            
            var player = ParsePlayers(handlines).FirstOrDefault(p => p.SeatNumber == heroSeatNumber);

            if (player != null)
            {
                return player.PlayerName;
            }
            return null;
        }

        private int GetWinningsLineNumberFromHandLines(string[] handLines)
        {
            // for total pot we need to sum up all winnings + rake
            for (int i = handLines.Length - 1; i > 0; i--)
            {
                // search for the win tag
                if (handLines[i].EndsWith("Win\">"))
                {
                    return i + 1;
                }
            }
            return -1;
        }

        protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistory)
        {
            // rake is at the end of the first line of a hand history
            string line = handLines[0];
            var rakeStartIndex = line.LastIndexOf('=') + 2;
            var rakeEndIndex = line.LastIndexOf('"');
            var rakeString = line.Substring(rakeStartIndex, rakeEndIndex - rakeStartIndex);

            handHistory.Rake = decimal.Parse(rakeString)/100.0m;
            handHistory.TotalPot = handHistory.Rake;

            int winningsLine = GetWinningsLineNumberFromHandLines(handLines);

            // sum up all winnings, amount starts at index 22
            for (int k = winningsLine; k < handLines.Length - 1; k++)
            {
                // leave the loop on </Action>
                if (handLines[k][1] == '/')
                {
                    break;
                }
                string amountString = handLines[k].Substring(22, handLines[k].IndexOf('"', 22) - 22);
                handHistory.TotalPot += decimal.Parse(amountString);
            }
        }
    }
}
