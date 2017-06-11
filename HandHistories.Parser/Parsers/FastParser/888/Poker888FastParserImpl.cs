using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Time;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.Uncalled;
using HandHistories.Parser.Utils.FastParsing;

namespace HandHistories.Parser.Parsers.FastParser._888
{
    public sealed class Poker888FastParserImpl : HandHistoryParserFastImpl
    {
        static readonly NumberFormatInfo NumberFormat2 = new NumberFormatInfo()
        {
            NumberDecimalSeparator = ",",
        };

        public override SiteName SiteName
        {
            get { return SiteName.Pacific; }
        }

        public override bool RequiresAllInDetection
        {
            get { return true; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
        }

        public override bool RequiresUncalledBetFix
        {
            get { return true; }
        }

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            rawHandHistories = rawHandHistories.Replace("\r", "");

            //This was causing an OOM exception so used LazyStringSplit
            //List<string> splitUpHands = rawHandHistories.Split(new char[] {'▄'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            //return splitUpHands.Where(s => s.Equals("\r\n") == false);

            return rawHandHistories.LazyStringSplit("\n\n").Where(s => !string.IsNullOrWhiteSpace(s) && !s.Equals("\r\n") && s.Length > 60);
        }

        private static readonly Regex DealerPositionRegex = new Regex(@"(?<=Seat )\d+", RegexOptions.Compiled);
        protected override int ParseDealerPosition(string[] handLines)
        {
            int start = GetHandStartIndex(handLines);
            return Int32.Parse(DealerPositionRegex.Match(handLines[start + 3]).Value);
        }

        private static readonly Regex DateRegex = new Regex(@"(\d+) (\d+) (\d+) ", RegexOptions.Compiled);
        protected override DateTime ParseDateUtc(string[] handLines)
        {
            int start = GetHandStartIndex(handLines);
            string line = handLines[start + 1];
            //Date looks like: $0.50/$1 Blinds No Limit Holdem - *** 06 01 2014 10:56:05
            int dateStartIndex = line.LastIndexOf('*') + 2;
            string dateString = line.Substring(dateStartIndex);
            //Change string so it becomes 2012-02-04 23:59:48
            dateString = DateRegex.Replace(dateString, "$3-$2-$1 ");

            DateTime dateTime = DateTime.Parse(dateString);

            //previously we converted the timezone but it seem Timezone is local and not always GMT
            //DateTime utcTime = TimeZoneUtil.ConvertDateTimeToUtc(dateTime, TimeZoneType.GMT);

            return dateTime;
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            return PokerFormat.CashGame;
        }

        protected override long ParseHandId(string[] handLines)
        {
            int start = GetHandStartIndex(handLines);
            string line = handLines[start];
            int endIndex = line.LastIndexOf(' ');
            int startIndex = line.LastIndexOf(' ', endIndex - 1);

            string idString = line.Substring(startIndex, endIndex - startIndex);
            return long.Parse(idString);
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        private static readonly Regex TableNameRegex = new Regex(@"(?<=Table ).*$", RegexOptions.Compiled);
        protected override string ParseTableName(string[] handLines)
        {
            int start = GetHandStartIndex(handLines);
            //"Table Athens 10 Max (Real Money)" -> "Athens"
            var tableName = TableNameRegex.Match(handLines[start + 2]).Value;
            tableName = tableName.Substring(0, tableName.Length - 19).TrimEnd();
            return tableName;
        }

        private static readonly Regex NumPlayersRegex = new Regex(@"(?<=Total number of players : )\d+", RegexOptions.Compiled);
        protected override SeatType ParseSeatType(string[] handLines)
        {
            int seatCount = ParsePlayerCount(handLines);

            if (seatCount <= 2)
            {
                return SeatType.FromMaxPlayers(2);
            }
            else if (seatCount <= 6)
            {
                return SeatType.FromMaxPlayers(6);
            }
            else if (seatCount <= 9)
            {
                return SeatType.FromMaxPlayers(9);
            }
            else
            {
                return SeatType.FromMaxPlayers(10);
            }
        }

        private static readonly Regex GameTypeRegex = new Regex(@"(?<=Blinds ).*(?= - )", RegexOptions.Compiled);
        protected override GameType ParseGameType(string[] handLines)
        {
            int start = GetHandStartIndex(handLines);
            string gameTypeString = GameTypeRegex.Match(handLines[start + 1]).Value;
            gameTypeString = gameTypeString.Replace(" Jackpot table", "");
            switch (gameTypeString)
            {
                case "No Limit Holdem":
                case "No Limit Holdem Jackpot table":
                    return GameType.NoLimitHoldem;
                case "Fix Limit Holdem":
                    return GameType.FixedLimitHoldem;
                case "Pot Limit Holdem":
                    return GameType.PotLimitHoldem;     
                case "Pot Limit Omaha":
                    return GameType.PotLimitOmaha;      
                case "No Limit Omaha":
                    return GameType.NoLimitOmaha;         
                case "Pot Limit OmahaHL":
                    return GameType.PotLimitOmahaHiLo;
                default:
                    throw new UnrecognizedGameTypeException(gameTypeString, "");
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            // 888 does not store any information about the tabletype in the hand history

            // we assume the table is push or fold if
            // - there is at least one player with exactly 5bb
            // - the average stack is < 17.5bb 
            // - at least two players have < 10bb
            // - there is no player with > 20bb

            // the min buyin for standard table is > 30bb, so this should work in most cases
            // furthermore if on a regular table the average stack is < 17.5, the play is just like on a push fold table and vice versa
            int start = GetHandStartIndex(handLines);
            bool isjackPotTable = handLines[start + 1].Contains(" Jackpot table");
           
            var playerList = ParsePlayers(handLines);
            var limit = ParseLimit(handLines);

            var tableStack = 0m;
            var playersBelow10bb = 0;
            var playersAbove20bb = 0;
            foreach (Player player in playerList)
            {
                tableStack += player.StartingStack;
                if (player.StartingStack / limit.BigBlind == 5m) return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);
                if (player.StartingStack / limit.BigBlind <= 10m) playersBelow10bb++;
                if (player.StartingStack / limit.BigBlind > 29m) playersAbove20bb++;

                if (playersBelow10bb > 1) return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);
            }

            if (playersAbove20bb == 0) return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);

            if (tableStack / limit.BigBlind / playerList.Count <= 17.5m)
            {
                if (isjackPotTable)
                {
                    return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold, TableTypeDescription.Jackpot);
                }
                return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);
            }
            if (isjackPotTable)
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Jackpot);
            }
            else
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
            }
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            int start = GetHandStartIndex(handLines);
            string line = handLines[start + 1];

            int LimitEndIndex = line.IndexOfFast(" Blinds");
            string limitString = line.Remove(LimitEndIndex);

            int splitIndex = limitString.IndexOf('/');

            string lowLimitString = limitString.Remove(splitIndex);
            string highLimitString = limitString.Substring(splitIndex + 1);

            decimal lowLimit = ParseAmount(lowLimitString);
            decimal highLimit = ParseAmount(highLimitString);

            return Limit.FromSmallBlindBigBlind(lowLimit, highLimit, Currency.USD);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidHand(string[] handLines)
        {
            const string Collected = " collected ";
            return handLines[handLines.Length - 1].Contains(Collected) || handLines[handLines.Length - 2].Contains(Collected);
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;

            int start = GetHandStartIndex(handLines);
            var seatedPlayersLine = handLines[start + 5];
            if (seatedPlayersLine[seatedPlayersLine.Length - 1] == '1')
            {
                isCancelled = true;
            }

            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType)
        {
            int actionIndex = GetFirstActionIndex(handLines);

            List<HandAction> handActions = new List<HandAction>(handLines.Length - actionIndex);

            actionIndex = ParseBlindActions(handLines, ref handActions, actionIndex);

            Street currentStreet = Street.Preflop;

            for (int i = actionIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];

                char firstChar = line[0];
                if (firstChar == '*')
                {
                    if (line[3] == 'S')
                    {
                        currentStreet = Street.Showdown;
                        actionIndex = i;
                        break;
                    }

                    switch (line[11])
                    {
                        case 'r':
                            currentStreet = Street.River;
                            break;
                        case 'f':
                            currentStreet = Street.Flop;
                            break;
                        case 't':
                            currentStreet = Street.Turn;
                            break;
                    }
                    
                    continue;
                }
                else if (firstChar == 'D' && line.StartsWithFast("Dealt "))
                {
                    continue;
                }

                HandAction action = ParseRegularActionLine(line, currentStreet);

                handActions.Add(action);
            }

            if (currentStreet == Street.Showdown)
            {
                for (int i = actionIndex; i < handLines.Length; i++)
                {
                    string line = handLines[i];

                    // ignore lines such as:
                    //  OprahTiltfre did not show his hand
                    if (line[line.Length - 1] != ']')
                    {
                        continue;
                    }

                    var action = ParseShowdownActionLine(line);
                    if (action != null)
                    {
                        handActions.Add(action);
                    }
                }
            }
            
            return handActions;
        }

        static readonly char[] suits = new char[]
        {
            's', 
            'h', 
            'c', 
            'd'
        };

        static bool ContainsCards(string bracketStr)
        {
            return bracketStr.IndexOfAny(suits) != -1;
        }

        public static HandAction ParseShowdownActionLine(string line)
        {
            int openSquareIndex = line.LastIndexOf('[');

            string bracketStr = line.Substring(openSquareIndex + 1, line.Length - openSquareIndex - 2);
            // winnings hands have numbers such as:
            //  OprahTiltfre collected [ $2,500 ]
            if (!ContainsCards(bracketStr))
            {
                string amountString = line.Substring(openSquareIndex + 1, line.Length - openSquareIndex - 1 - 1);
                decimal amount = ParseAmount(amountString);

                string playerName = line.Substring(0, openSquareIndex - 11);

                return new WinningsAction(playerName, HandActionType.WINS, amount, 0);
            }

            string action = line.Substring(openSquareIndex - 6, 5);
            if (action.Equals("shows"))
            {
                string playerName = line.Substring(0, openSquareIndex - 7);
                return new HandAction(playerName, HandActionType.SHOW, 0, Street.Showdown);
            }
            else if (action.Equals("mucks"))
            {
                string playerName = line.Substring(0, openSquareIndex - 7);
                return new HandAction(playerName, HandActionType.MUCKS, 0, Street.Showdown);
            }

            throw new HandActionException(line, "Unparsed");
        }

        public static HandAction ParseRegularActionLine(string line, Street currentStreet)
        {
            if (line[line.Length - 1] == ']')
            {
                int openSquareIndex = line.LastIndexOf('[');
                string amountString = line.Substring(openSquareIndex + 1, line.Length - openSquareIndex - 1 - 1);

                decimal amount = ParseAmount(amountString);
                string action = line.Substring(openSquareIndex - 8, 7);

                if (action.EndsWithFast(" raises"))
                {
                    string playerName = line.Substring(0, openSquareIndex - 8);
                    return new HandAction(playerName, HandActionType.RAISE, amount, currentStreet);
                }
                else if (action.EndsWithFast(" bets"))
                {
                    string playerName = line.Substring(0, openSquareIndex - 6);
                    return new HandAction(playerName, HandActionType.BET, amount, currentStreet);
                }
                else if (action.EndsWithFast(" calls"))
                {
                    string playerName = line.Substring(0, openSquareIndex - 7);
                    return new HandAction(playerName, HandActionType.CALL, amount, currentStreet);
                }
            }
            else if (line.EndsWithFast(" folds"))
            {
                string playerName = line.Substring(0, line.Length - 6);
                return new HandAction(playerName, HandActionType.FOLD, currentStreet);
            }
            else if (line.EndsWithFast(" checks"))
            {
                string playerName = line.Substring(0, line.Length - 7);
                return new HandAction(playerName, HandActionType.CHECK, currentStreet);
            }

            throw new HandActionException(line, "Unknown Player action line: " + line);
        }

        static int ParseBlindActions(string[] handLines, ref List<HandAction> handActions, int actionIndex)
        {
            for (int i = actionIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line[0] == '*')
                {
                    if (line.StartsWithFast("** Dealing down cards **"))
                    {
                        return i + 1;
                    }
                    if (line.StartsWithFast("** Dealing "))
                    {
                        return i;
                    }
                    else if (line == "** Summary **")
                    {
                        return i;
                    }
                }

                try
                {
                    var action = ParseBlindAction(line);
                    handActions.Add(action);
                }
                catch (HandActionException)
                {
                    var action = ParseRegularActionLine(line, Street.Preflop);
                    handActions.Add(action);
                }
                catch
                {
                    throw;
                }
            }
            throw new HandActionException(string.Join(Environment.NewLine, handLines), "No cards was dealt");
        }

        public static HandAction ParseBlindAction(string line)
        {
            const int smallBlindWidth = 19;//" posts small blind ".Length
            const int bigBlindWidth = 17;//" posts big blind ".Length
            const int anteWidth = 12;//" posts ante ".Length
            const int PostingWidth = 18;//" posts dead blind ".Length
            int openSquareIndex = line.LastIndexOf('[');
            string amountString = line.Substring(openSquareIndex + 1, line.Length - openSquareIndex - 2);

            //there may be folds during the blinds
            if (openSquareIndex == -1 || (line[openSquareIndex - 2] != 'd' && line[openSquareIndex - 2] != 'e'))
            {
                throw new HandActionException(line, "Not a blindAction");
            }

            string playerName;
            decimal amount;
            HandActionType actionType;
            var actionTypeChar = line[openSquareIndex - 8];
            switch (actionTypeChar)
            {
                case 'l':
                    actionType = HandActionType.SMALL_BLIND;
                    amount = ParseAmount(amountString);
                    playerName = line.Substring(0, openSquareIndex - smallBlindWidth);
                    break;
                case 'g':
                    actionType = HandActionType.BIG_BLIND;
                    amount = ParseAmount(amountString);
                    playerName = line.Substring(0, openSquareIndex - bigBlindWidth);
                    break;
                case 't':
                    actionType = HandActionType.ANTE;
                    amount = ParseAmount(amountString);
                    playerName = line.Substring(0, openSquareIndex - anteWidth);
                    break;
                case 'd':
                    actionType = HandActionType.POSTS;
                    amount = ParseDeadBlindAmount(amountString);
                    playerName = line.Substring(0, openSquareIndex - PostingWidth);
                    break;
                default:
                    throw new ArgumentException("Invalid Blind ActionType: " + line);
            }

            return new HandAction(playerName, actionType, amount, Street.Preflop);
        }

        static int GetFirstActionIndex(string[] handLines)
        {
            for (int i = 6; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[line.Length - 1] == ')')
                {
                    continue;
                }
                
                return i;
            }
            throw new ArgumentOutOfRangeException("Did not find FirstActionIndex");
        }

        private static decimal ParseAmount(string amountString)
        {
            if (amountString.Contains(','))
            {
                amountString = amountString.Trim(' ');
                if (amountString.Length > 5 && amountString[amountString.Length - 5] == ',')
                {
                    var chars = amountString.Where(p => char.IsDigit(p) || p == ',');
                    amountString = string.Concat(chars);

                    return amountString.ParseAmount(NumberFormat2);
                }
            }
            else if(amountString.Contains((char)160))
            {
                var chars = amountString.Where(p => char.IsDigit(p) || p == ',');
                amountString = string.Concat(chars);
                return amountString.ParseAmount();
            }

            return amountString.ParseAmountWS();
        }

        static decimal ParseDeadBlindAmount(string amountString)
        {
            int plusIndex = amountString.IndexOf('+');
            string amountStr1 = amountString.Remove(plusIndex);
            string amountStr2 = amountString.Substring(plusIndex + 1);

            decimal amount1 = ParseAmount(amountStr1);
            decimal amount2 = ParseAmount(amountStr2);

            return amount1 + amount2;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            const int PlayerListStartIndex = 5;
            int start = GetHandStartIndex(handLines);
            int seatCount = ParsePlayerCount(handLines);

            PlayerList playerList = new PlayerList();

            for (int i = 0; i < seatCount; i++)
            {
                string handLine = handLines[PlayerListStartIndex + start + i];

                // Expected format:
                //  Seat 1: Velmonio ( $1.05 )
                int colonIndex = handLine.IndexOf(':');
                int openParenIndex = handLine.IndexOf('(');

                int seat = int.Parse(handLine.Substring(5, colonIndex - 5));
                string playerName = handLine.Substring(colonIndex + 2, openParenIndex - colonIndex - 3);
                decimal amount = ParseAmount(handLine.Substring(openParenIndex + 1, handLine.Length - openParenIndex - 1 - 2));

                playerList.Add(new Player(playerName, amount, seat));
            }

            int heroCardsIndex = GetHeroCardsIndex(handLines, PlayerListStartIndex + seatCount);

            if (heroCardsIndex != -1)
            {
                string heroCardsLine = handLines[heroCardsIndex];
                if (heroCardsLine[heroCardsLine.Length - 1] == ']' &&
                    heroCardsLine.StartsWithFast("Dealt to "))
                {
                    int openSquareIndex = heroCardsLine.LastIndexOf('[');

                    string cards = heroCardsLine.Substring(openSquareIndex + 2, heroCardsLine.Length - openSquareIndex - 2 - 2);
                    HoleCards holeCards = HoleCards.FromCards(FixPacificCards(cards));

                    string playerName = heroCardsLine.Substring(9, openSquareIndex - 1 - 9);

                    Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                    player.HoleCards = holeCards;
                }
            }
            
            // Add hole-card info
            for (int i = handLines.Length - 2; i >= 0; i--)
            {
                string handLine = handLines[i];

                if (handLine[0] == '*')
                {
                    break;                    
                }

                if (!handLine.EndsWith("]"))
	            {
		            continue;
                }

                int openSquareIndex = handLine.LastIndexOf('[');
                string bracketStr = handLine.Substring(openSquareIndex + 2, handLine.Length - openSquareIndex - 2 - 2);
                if (ContainsCards(bracketStr))
                {
                    // lines such as:
                    //  slyguyone2 shows [ Jd, As ]
                    string cards = handLine.Substring(openSquareIndex + 2, handLine.Length - openSquareIndex - 2 - 2);
                    HoleCards holeCards = HoleCards.FromCards(FixPacificCards(cards));

                    string playerName = handLine.Substring(0, openSquareIndex - 7);

                    Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                    player.HoleCards = holeCards;
                }
            }

            return playerList;
        }

        static string FixPacificCards(string str)
        {
            return str.Replace(",", "")
                .Replace(" ", "")
                .Replace("Kn", "J")
                .Replace("D", "Q")
                .Replace("E", "A");
        }

        private static int ParsePlayerCount(string[] handLines)
        {
            int start = GetHandStartIndex(handLines);
            int seatCount = FastInt.Parse(NumPlayersRegex.Match(handLines[start + 4]).Value);
            return seatCount;
        }

        static int GetHeroCardsIndex(string[] handLines, int startIndex)
        {
            for (int i = startIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[0] == '*' && line[line.Length - 1] == '*')
                {
                    return i + 1;
                }
            }
            return -1;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;

            for (int i = 0; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine[0] != '*')
                {
                    continue;                    
                }

                if (handLine[3] != 'D')
                {
                    continue;
                }

                int openSquareIndex;
                switch (handLine[11])
                {
                    case 'r':
                        openSquareIndex = 20;
                        break;
                    default:
                        openSquareIndex = 19;
                        break;                        
                }

                string cards = handLine.Substring(openSquareIndex + 2, handLine.Length - openSquareIndex - 2 - 2);

                boardCards += FixPacificCards(cards);
            }

            return BoardCards.FromCards(boardCards);
        }

        protected override string ParseHeroName(string[] handlines)
        {
            const string DealText = "Dealt to ";
            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][0] == 'D' && handlines[i].StartsWith(DealText))
                {
                    string line = handlines[i];
                    int startindex = DealText.Length;
                    int endindex = line.LastIndexOf('[') - 1;
                    return line.Substring(startindex, endindex - startindex);
                }
            }
            return null;
        }

        static int GetHandStartIndex(string[] handlines)
        {
            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][0] == '*')
                {
                    return i;
                }
            }
            throw new ArgumentException("Did not find start of hand");
        }
    }
}
